namespace AtomUI.Modularity;

public sealed class ModuleCatalog
{
    private readonly IReadOnlyList<ModuleDescriptor> _modules;
    private readonly IReadOnlyDictionary<ModuleId, ModuleDescriptor> _modulesById;

    public ModuleCatalog(IEnumerable<ModuleDescriptor> modules)
    {
        ArgumentNullException.ThrowIfNull(modules);

        var moduleArray = modules.ToArray();
        ThrowIfDuplicateIds(moduleArray);

        var modulesById = new Dictionary<ModuleId, ModuleDescriptor>();

        foreach (var module in moduleArray)
        {
            modulesById.Add(module.Id, module);
        }

        _modules = Array.AsReadOnly(moduleArray);
        _modulesById = modulesById;
    }

    public static ModuleCatalog Empty { get; } = new([]);

    public IReadOnlyList<ModuleDescriptor> Modules => _modules;

    public bool TryGetModule<TModule>(out ModuleDescriptor? module)
        where TModule : IModule
    {
        return TryGetModule(ModuleId.FromModuleType<TModule>(), out module);
    }

    internal bool TryGetModule(ModuleId moduleId, out ModuleDescriptor? module)
    {
        return _modulesById.TryGetValue(moduleId, out module);
    }

    public ModuleResolutionResult Resolve()
    {
        return ModuleDependencyGraph.TryResolve(this);
    }

    private static void ThrowIfDuplicateIds(IReadOnlyList<ModuleDescriptor> modules)
    {
        var duplicate = modules
            .Select(module => module.Id)
            .GroupBy(moduleId => moduleId)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicate is null)
        {
            return;
        }

        throw new ModuleGraphException(
        [
            new ModuleDiagnostic(
                ModuleDiagnosticCodes.DuplicateModuleId,
                $"Module id or alias '{duplicate.Key}' is declared more than once.",
                duplicate.Key)
        ]);
    }
}
