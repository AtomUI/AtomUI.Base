namespace AtomUI.Modularity;

public sealed class ModuleHostOptionsBuilder
{
    private readonly HashSet<ModuleId> _enabledModuleIds = [];
    private IModuleDiagnostics? _diagnostics;

    public ModuleHostOptionsBuilder Enable<TModule>()
        where TModule : IModule
    {
        _enabledModuleIds.Add(ModuleId.FromModuleType<TModule>());
        return this;
    }

    public ModuleHostOptionsBuilder UseDiagnostics(IModuleDiagnostics diagnostics)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);

        _diagnostics = diagnostics;
        return this;
    }

    public ModuleHostOptions Build()
    {
        return new ModuleHostOptions(_enabledModuleIds, _diagnostics);
    }
}
