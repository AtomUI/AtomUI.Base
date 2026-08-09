using System.Collections.ObjectModel;

namespace AtomUI.Modularity;

public sealed class ModuleHost
{
    private readonly IReadOnlyList<ModuleRegistration> _registrations;
    private readonly ModuleHostOptions _options;
    private readonly List<ModuleEntry> _enabledModules = [];
    private readonly ReadOnlyCollection<ModuleEntry> _enabledModuleView;
    private readonly ModuleServiceCollection _services = new();
    private IReadOnlyList<ModuleDescriptor>? _resolvedModules;
    private ModuleHostPhase _phase = ModuleHostPhase.Created;
    private bool _isInitialized;
    private bool _isShutdown;

    public ModuleHost(IEnumerable<ModuleRegistration> registrations, ModuleHostOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(registrations);

        _registrations = Array.AsReadOnly(registrations.ToArray());
        _options = options ?? new ModuleHostOptions();
        _enabledModuleView = _enabledModules.AsReadOnly();
        Services = new ModuleServiceRegistry(_services);
    }

    public IReadOnlyList<ModuleEntry> EnabledModules => _enabledModuleView;

    public ModuleHostPhase Phase => _phase;

    public ModuleServiceRegistry Services { get; }

    public async ValueTask<ModuleHostResult> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var terminalResult = RequireNotTerminal("Initialize");
        if (!terminalResult.IsSuccess)
        {
            return terminalResult;
        }

        if (_isInitialized)
        {
            return ModuleHostResult.Success;
        }

        try
        {
            var result = ResolveModules();
            if (!result.IsSuccess)
            {
                return result;
            }

            result = await CreateModulesAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                return result;
            }

            result = await PreConfigureServicesAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                return await RollBackFailedInitializationAsync(result, CancellationToken.None);
            }

            result = await ConfigureServicesAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                return await RollBackFailedInitializationAsync(result, CancellationToken.None);
            }

            result = await PostConfigureServicesAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                return await RollBackFailedInitializationAsync(result, CancellationToken.None);
            }

            result = FreezeServices();
            if (!result.IsSuccess)
            {
                return await RollBackFailedInitializationAsync(result, CancellationToken.None);
            }

            result = await InitializeModulesAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                return await RollBackFailedInitializationAsync(result, CancellationToken.None);
            }

            _isInitialized = true;
            return ModuleHostResult.Success;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return await RollBackFailedInitializationAsync(
                CreateResult(
                [
                    new ModuleDiagnostic(
                        ModuleDiagnosticCodes.ModuleLifecycleFailed,
                        exception.Message)
                ]),
                CancellationToken.None);
        }
    }

    public ModuleHostResult ResolveModules()
    {
        var terminalResult = RequireNotTerminal("ResolveModules");
        if (!terminalResult.IsSuccess)
        {
            return terminalResult;
        }

        if (_resolvedModules is not null)
        {
            return ModuleHostResult.Success;
        }

        var registrationDiagnostics = ValidateRegistrations();
        if (registrationDiagnostics.Count > 0)
        {
            return CreateResult(registrationDiagnostics);
        }

        var selectedDescriptors = SelectEnabledDescriptors();
        var graphResult = ModuleDependencyGraph.TryResolve(selectedDescriptors);
        if (!graphResult.IsSuccess)
        {
            return CreateResult(graphResult.Diagnostics);
        }

        _resolvedModules = graphResult.Modules;
        _phase = ModuleHostPhase.Resolved;
        return ModuleHostResult.Success;
    }

    public ModuleHostResult CreateModules()
    {
        return CreateModulesAsync(CancellationToken.None).AsTask().GetAwaiter().GetResult();
    }

    private async ValueTask<ModuleHostResult> CreateModulesAsync(CancellationToken cancellationToken)
    {
        var terminalResult = RequireNotTerminal("CreateModules");
        if (!terminalResult.IsSuccess)
        {
            return terminalResult;
        }

        var resolveResult = ResolveModules();
        if (!resolveResult.IsSuccess)
        {
            return resolveResult;
        }

        if (_phase == ModuleHostPhase.ModulesCreated)
        {
            return ModuleHostResult.Success;
        }

        var phaseResult = RequirePhase(ModuleHostPhase.Resolved, "CreateModules");
        if (!phaseResult.IsSuccess)
        {
            return phaseResult;
        }

        var created = new List<ModuleEntry>();
        try
        {
            foreach (var descriptor in _resolvedModules ?? [])
            {
                cancellationToken.ThrowIfCancellationRequested();
                var registration = _registrations.First(item => item.Descriptor.Id == descriptor.Id);
                created.Add(new ModuleEntry(descriptor, CreateModule(registration)));
            }

            _enabledModules.AddRange(created);
            _phase = ModuleHostPhase.ModulesCreated;
            return ModuleHostResult.Success;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ModuleLifecycleExecutionException exception)
        {
            var diagnostics = new List<ModuleDiagnostic>
            {
                CreateLifecycleFailureDiagnostic(exception)
            };
            diagnostics.AddRange(await ShutdownEntriesAsync(created, CancellationToken.None));
            created.Clear();
            _enabledModules.Clear();
            _phase = ModuleHostPhase.Failed;
            _isShutdown = true;
            return CreateResult(diagnostics);
        }
    }

    public ValueTask<ModuleHostResult> PreConfigureServicesAsync(CancellationToken cancellationToken = default)
    {
        return RunServicePhaseAsync(
            "PreConfigureServices",
            ModuleHostPhase.ModulesCreated,
            ModuleHostPhase.PreConfiguredServices,
            (module, context, token) => module.PreConfigureServicesAsync(context, token),
            cancellationToken);
    }

    public ValueTask<ModuleHostResult> ConfigureServicesAsync(CancellationToken cancellationToken = default)
    {
        return RunServicePhaseAsync(
            "ConfigureServices",
            ModuleHostPhase.PreConfiguredServices,
            ModuleHostPhase.ConfiguredServices,
            (module, context, token) => module.ConfigureServicesAsync(context, token),
            cancellationToken);
    }

    public ValueTask<ModuleHostResult> PostConfigureServicesAsync(CancellationToken cancellationToken = default)
    {
        return RunServicePhaseAsync(
            "PostConfigureServices",
            ModuleHostPhase.ConfiguredServices,
            ModuleHostPhase.PostConfiguredServices,
            (module, context, token) => module.PostConfigureServicesAsync(context, token),
            cancellationToken);
    }

    public ModuleHostResult FreezeServices()
    {
        var phaseResult = RequirePhase(ModuleHostPhase.PostConfiguredServices, "FreezeServices");
        if (!phaseResult.IsSuccess)
        {
            return phaseResult;
        }

        _services.Freeze();
        foreach (var entry in _enabledModules)
        {
            entry.Phase = ModuleHostPhase.ServicesFrozen;
        }
        _phase = ModuleHostPhase.ServicesFrozen;
        return ModuleHostResult.Success;
    }

    public async ValueTask<ModuleHostResult> InitializeModulesAsync(CancellationToken cancellationToken = default)
    {
        var phaseResult = RequirePhase(ModuleHostPhase.ServicesFrozen, "Initialize");
        if (!phaseResult.IsSuccess)
        {
            return phaseResult;
        }

        try
        {
            foreach (var entry in _enabledModules)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ModuleInitializationContext(entry.Descriptor, Services);
                await InvokeModuleAsync(
                    entry,
                    "Initialize",
                    (module, token) => module.InitializeAsync(context, token),
                    cancellationToken);
                entry.Phase = ModuleHostPhase.Initialized;
            }

            _isInitialized = true;
            _phase = ModuleHostPhase.Initialized;
            return ModuleHostResult.Success;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ModuleLifecycleExecutionException exception)
        {
            return await CreateLifecycleFailureResultAsync(exception);
        }
    }

    public async ValueTask<ModuleHostResult> RunModulePhaseAsync(
        ModulePhaseId phaseId,
        ModuleHostPhase requiredPhase,
        ModulePhaseExecutionOrder executionOrder,
        Func<ModulePhaseContext, CancellationToken, ValueTask> phase,
        CancellationToken cancellationToken = default,
        bool rollbackOnFailure = true)
    {
        return await RunModulePhaseCoreAsync(
            phaseId.Value,
            requiredPhase,
            executionOrder,
            phase,
            cancellationToken,
            rollbackOnFailure);
    }

    private async ValueTask<ModuleHostResult> RunModulePhaseCoreAsync(
        string stage,
        ModuleHostPhase requiredPhase,
        ModulePhaseExecutionOrder executionOrder,
        Func<ModulePhaseContext, CancellationToken, ValueTask> phase,
        CancellationToken cancellationToken,
        bool rollbackOnFailure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stage);
        ArgumentNullException.ThrowIfNull(phase);
        if (!Enum.IsDefined(executionOrder))
        {
            throw new ArgumentOutOfRangeException(
                nameof(executionOrder),
                executionOrder,
                "Module phase execution order is not supported.");
        }

        var terminalResult = RequireNotTerminal(stage);
        if (!terminalResult.IsSuccess)
        {
            return terminalResult;
        }

        var phaseResult = RequirePhase(requiredPhase, stage);
        if (!phaseResult.IsSuccess)
        {
            return phaseResult;
        }

        var entries = executionOrder == ModulePhaseExecutionOrder.Forward
            ? _enabledModules
            : _enabledModules.AsEnumerable().Reverse();

        try
        {
            foreach (var entry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ModulePhaseContext(entry.Descriptor, Services);
                await InvokeModulePhaseAsync(entry, stage, phase, context, cancellationToken);
            }

            return ModuleHostResult.Success;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ModuleLifecycleExecutionException exception)
        {
            return rollbackOnFailure
                ? await CreateLifecycleFailureResultAsync(exception)
                : CreateLifecycleFailureResult(exception);
        }
    }

    private async ValueTask<ModuleHostResult> RollBackFailedInitializationAsync(
        ModuleHostResult result,
        CancellationToken cancellationToken)
    {
        var rollbackResult = await ShutdownAsync(cancellationToken);
        _isShutdown = true;
        _phase = ModuleHostPhase.Failed;

        return rollbackResult.IsSuccess
            ? result
            : new ModuleHostResult(result.Diagnostics.Concat(rollbackResult.Diagnostics));
    }

    private ModuleHostResult RollBackFailedInitialization(ModuleHostResult result)
    {
        return RollBackFailedInitializationAsync(result, CancellationToken.None)
            .AsTask()
            .GetAwaiter()
            .GetResult();
    }

    public async ValueTask<ModuleHostResult> ShutdownAsync(CancellationToken cancellationToken = default)
    {
        if (_isShutdown)
        {
            return ModuleHostResult.Success;
        }

        var diagnostics = await ShutdownEntriesAsync(_enabledModules, cancellationToken);

        _isShutdown = true;
        _phase = ModuleHostPhase.Shutdown;
        return diagnostics.Count == 0
            ? ModuleHostResult.Success
            : CreateResult(diagnostics);
    }

    private async ValueTask<IReadOnlyList<ModuleDiagnostic>> ShutdownEntriesAsync(
        IReadOnlyList<ModuleEntry> entries,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<ModuleDiagnostic>();

        for (var index = entries.Count - 1; index >= 0; index--)
        {
            var entry = entries[index];
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ModuleShutdownContext(entry.Descriptor, Services);
                await InvokeModuleAsync(
                    entry,
                    "Shutdown",
                    (module, token) => module.ShutdownAsync(context, token),
                    cancellationToken);
                entry.Phase = ModuleHostPhase.Shutdown;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ModuleLifecycleExecutionException exception)
            {
                diagnostics.Add(CreateLifecycleFailureDiagnostic(exception));
            }
            catch (Exception exception)
            {
                diagnostics.Add(new ModuleDiagnostic(
                    ModuleDiagnosticCodes.ModuleLifecycleFailed,
                    exception.Message,
                    entry.Descriptor.Id));
            }
        }

        return diagnostics;
    }

    private async ValueTask<ModuleHostResult> RunServicePhaseAsync(
        string stage,
        ModuleHostPhase expectedPhase,
        ModuleHostPhase nextPhase,
        Func<IModule, ModuleServiceConfigurationContext, CancellationToken, ValueTask> phase,
        CancellationToken cancellationToken)
    {
        var phaseResult = RequirePhase(expectedPhase, stage);
        if (!phaseResult.IsSuccess)
        {
            return phaseResult;
        }

        try
        {
            foreach (var entry in _enabledModules)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var context = new ModuleServiceConfigurationContext(entry.Descriptor, _services);
                await InvokeModuleAsync(
                    entry,
                    stage,
                    (module, token) => phase(module, context, token),
                    cancellationToken);
                entry.Phase = nextPhase;
            }

            _phase = nextPhase;
            return ModuleHostResult.Success;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ModuleLifecycleExecutionException exception)
        {
            return await CreateLifecycleFailureResultAsync(exception);
        }
    }

    private static async ValueTask InvokeModuleAsync(
        ModuleEntry entry,
        string stage,
        Func<IModule, CancellationToken, ValueTask> action,
        CancellationToken cancellationToken)
    {
        try
        {
            await action(entry.Module, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new ModuleLifecycleExecutionException(entry.Descriptor.Id, stage, exception);
        }
    }

    private static async ValueTask InvokeModulePhaseAsync(
        ModuleEntry entry,
        string stage,
        Func<ModulePhaseContext, CancellationToken, ValueTask> action,
        ModulePhaseContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            await action(context, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new ModuleLifecycleExecutionException(entry.Descriptor.Id, stage, exception);
        }
    }

    private IReadOnlyList<ModuleDescriptor> SelectEnabledDescriptors()
    {
        var selected = new Dictionary<ModuleId, ModuleDescriptor>();
        var registrationsById = BuildRegistrationMap(_registrations);

        foreach (var registration in _registrations)
        {
            if (IsEnabled(registration.Descriptor))
            {
                AddWithRequiredDependencies(registration.Descriptor);
            }
        }

        return Array.AsReadOnly(selected.Values.ToArray());

        void AddWithRequiredDependencies(ModuleDescriptor descriptor)
        {
            if (!selected.TryAdd(descriptor.Id, descriptor))
            {
                return;
            }

            foreach (var dependency in descriptor.Dependencies)
            {
                if (registrationsById.TryGetValue(dependency.ModuleId, out var registration))
                {
                    AddWithRequiredDependencies(registration.Descriptor);
                }
            }
        }

        bool IsEnabled(ModuleDescriptor descriptor)
        {
            return _options.EnabledModuleIds.Contains(descriptor.Id);
        }
    }

    private IReadOnlyList<ModuleDiagnostic> ValidateRegistrations()
    {
        var duplicate = _registrations
            .Select(registration => registration.Descriptor.Id)
            .GroupBy(moduleId => moduleId)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate is not null)
        {
            return
            [
                new ModuleDiagnostic(
                    ModuleDiagnosticCodes.DuplicateModuleId,
                    $"Module id '{duplicate.Key}' is registered more than once.",
                    duplicate.Key)
            ];
        }

        var registeredModuleIds = BuildRegistrationMap(_registrations).Keys;
        foreach (var enabledModuleId in _options.EnabledModuleIds)
        {
            if (!registeredModuleIds.Contains(enabledModuleId))
            {
                return
                [
                    new ModuleDiagnostic(
                        ModuleDiagnosticCodes.RequiredDependencyMissing,
                        $"Enabled module '{enabledModuleId}' is not registered.",
                        enabledModuleId)
                ];
            }
        }

        return [];
    }

    private static IReadOnlyDictionary<ModuleId, ModuleRegistration> BuildRegistrationMap(
        IEnumerable<ModuleRegistration> registrations)
    {
        var registrationsById = new Dictionary<ModuleId, ModuleRegistration>();
        foreach (var registration in registrations)
        {
            registrationsById.TryAdd(registration.Descriptor.Id, registration);
        }

        return registrationsById;
    }

    private static IModule CreateModule(ModuleRegistration registration)
    {
        try
        {
            return registration.Factory();
        }
        catch (Exception exception)
        {
            throw new ModuleLifecycleExecutionException(
                registration.Descriptor.Id,
                "CreateModule",
                exception);
        }
    }

    private ModuleHostResult RequirePhase(ModuleHostPhase expectedPhase, string stage)
    {
        return _phase == expectedPhase
            ? ModuleHostResult.Success
            : CreateInvalidLifecycleTransitionResult(stage, expectedPhase.ToString());
    }

    private ModuleHostResult RequireNotTerminal(string stage)
    {
        return _phase is not (ModuleHostPhase.Failed or ModuleHostPhase.Shutdown)
            ? ModuleHostResult.Success
            : CreateInvalidLifecycleTransitionResult(stage, "a non-terminal phase");
    }

    private ModuleHostResult CreateInvalidLifecycleTransitionResult(
        string stage,
        string expectedPhase)
    {
        return CreateResult(
        [
            new ModuleDiagnostic(
                ModuleDiagnosticCodes.InvalidLifecycleTransition,
                $"Module host cannot run '{stage}' while current phase is '{_phase}'. Expected phase '{expectedPhase}'.",
                stage: stage,
                context: new Dictionary<string, string>
                {
                    ["currentPhase"] = _phase.ToString(),
                    ["expectedPhase"] = expectedPhase
                })
        ]);
    }

    private ModuleHostResult CreateLifecycleFailureResult(ModuleLifecycleExecutionException exception)
    {
        return CreateResult([CreateLifecycleFailureDiagnostic(exception)]);
    }

    private async ValueTask<ModuleHostResult> CreateLifecycleFailureResultAsync(
        ModuleLifecycleExecutionException exception)
    {
        return await RollBackFailedInitializationAsync(
            CreateLifecycleFailureResult(exception),
            CancellationToken.None);
    }

    private ModuleDiagnostic CreateLifecycleFailureDiagnostic(ModuleLifecycleExecutionException exception)
    {
        var descriptor = (_resolvedModules ?? _registrations.Select(registration => registration.Descriptor))
            .FirstOrDefault(item => item.Id == exception.ModuleId);

        return new ModuleDiagnostic(
            ModuleDiagnosticCodes.ModuleLifecycleFailed,
            $"Module '{exception.ModuleId}' failed during '{exception.Stage}': {exception.InnerException?.Message ?? exception.Message}",
            moduleId: exception.ModuleId,
            stage: exception.Stage,
            sourceAssemblyName: descriptor?.SourceAssemblyName,
            exceptionType: exception.InnerException?.GetType().FullName);
    }

    private ModuleHostResult CreateResult(IEnumerable<ModuleDiagnostic> diagnostics)
    {
        var diagnosticArray = diagnostics.ToArray();
        foreach (var diagnostic in diagnosticArray)
        {
            _options.Diagnostics?.Report(diagnostic);
        }

        return new ModuleHostResult(diagnosticArray);
    }

    private sealed class ModuleLifecycleExecutionException(
        ModuleId moduleId,
        string stage,
        Exception innerException)
        : Exception(innerException.Message, innerException)
    {
        public ModuleId ModuleId { get; } = moduleId;

        public string Stage { get; } = stage;
    }
}
