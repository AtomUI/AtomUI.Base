namespace AtomUI.Modularity;

public sealed class ModuleDependencyGraph
{
    private ModuleDependencyGraph(IReadOnlyList<ModuleDescriptor> modules)
    {
        Modules = Array.AsReadOnly(modules.ToArray());
    }

    public IReadOnlyList<ModuleDescriptor> Modules { get; }

    public static ModuleDependencyGraph Resolve(IEnumerable<ModuleDescriptor> modules)
    {
        var result = TryResolve(modules);
        result.ThrowIfFailed();

        return new ModuleDependencyGraph(result.Modules);
    }

    public static ModuleDependencyGraph Resolve(ModuleCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        return Resolve(catalog.Modules);
    }

    public static ModuleResolutionResult TryResolve(ModuleCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        return TryResolve(catalog.Modules);
    }

    public static ModuleResolutionResult TryResolve(IEnumerable<ModuleDescriptor> modules)
    {
        ArgumentNullException.ThrowIfNull(modules);

        var moduleArray = modules.ToArray();
        var diagnostics = Validate(moduleArray);

        if (diagnostics.Count > 0)
        {
            return new ModuleResolutionResult(diagnostics: diagnostics);
        }

        var moduleMap = BuildModuleMap(moduleArray);
        var ordered = new List<ModuleDescriptor>();
        var states = new Dictionary<ModuleId, VisitState>();
        var path = new Stack<ModuleId>();

        try
        {
            foreach (var module in moduleArray)
            {
                Visit(module, moduleMap, states, ordered, path);
            }
        }
        catch (ModuleGraphException exception)
        {
            return new ModuleResolutionResult(diagnostics: exception.Diagnostics);
        }

        return new ModuleResolutionResult(ordered);
    }

    private static List<ModuleDiagnostic> Validate(IReadOnlyList<ModuleDescriptor> modules)
    {
        var diagnostics = new List<ModuleDiagnostic>();
        var duplicate = modules
            .Select(module => module.Id)
            .GroupBy(moduleId => moduleId)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate is not null)
        {
            diagnostics.Add(new ModuleDiagnostic(
                ModuleDiagnosticCodes.DuplicateModuleId,
                $"Module id '{duplicate.Key}' is declared more than once.",
                duplicate.Key));

            return diagnostics;
        }

        var moduleMap = BuildModuleMap(modules);

        foreach (var module in modules)
        {
            foreach (var dependency in module.Dependencies)
            {
                if (!moduleMap.TryGetValue(dependency.ModuleId, out _))
                {
                    diagnostics.Add(new ModuleDiagnostic(
                        ModuleDiagnosticCodes.RequiredDependencyMissing,
                        $"Module '{module.Id}' depends on missing module '{dependency.ModuleId}'.",
                        module.Id,
                        dependency.ModuleId));
                }
            }
        }

        return diagnostics;
    }

    private static void Visit(
        ModuleDescriptor module,
        IReadOnlyDictionary<ModuleId, ModuleDescriptor> moduleMap,
        IDictionary<ModuleId, VisitState> states,
        List<ModuleDescriptor> ordered,
        Stack<ModuleId> path)
    {
        if (states.TryGetValue(module.Id, out var state))
        {
            if (state == VisitState.Visited)
            {
                return;
            }

            throw new ModuleGraphException([
                new ModuleDiagnostic(
                    ModuleDiagnosticCodes.DependencyCycle,
                    $"Module dependency graph contains a cycle: {CreateCyclePath(path, module.Id)}.",
                    module.Id)
            ]);
        }

        states.Add(module.Id, VisitState.Visiting);
        path.Push(module.Id);

        foreach (var dependency in module.Dependencies)
        {
            if (moduleMap.TryGetValue(dependency.ModuleId, out var dependencyModule))
            {
                Visit(dependencyModule, moduleMap, states, ordered, path);
            }
        }

        path.Pop();
        states[module.Id] = VisitState.Visited;
        ordered.Add(module);
    }

    private static string CreateCyclePath(IEnumerable<ModuleId> path, ModuleId repeatedModuleId)
    {
        var orderedPath = path
            .Reverse()
            .Select(moduleId => moduleId.Value)
            .ToArray();
        var cycleStart = Array.IndexOf(orderedPath, repeatedModuleId.Value);

        if (cycleStart < 0)
        {
            return string.Join(" -> ", orderedPath.Append(repeatedModuleId.Value));
        }

        return string.Join(" -> ", orderedPath.Skip(cycleStart).Append(repeatedModuleId.Value));
    }

    private static IReadOnlyDictionary<ModuleId, ModuleDescriptor> BuildModuleMap(
        IReadOnlyList<ModuleDescriptor> modules)
    {
        var moduleMap = new Dictionary<ModuleId, ModuleDescriptor>();
        foreach (var module in modules)
        {
            moduleMap.TryAdd(module.Id, module);
        }

        return moduleMap;
    }

    private enum VisitState
    {
        Visiting,
        Visited
    }
}
