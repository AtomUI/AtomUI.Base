namespace AtomUI.Modularity;

public enum ModuleHostPhase
{
    Created,
    Resolved,
    ModulesCreated,
    PreConfiguredServices,
    ConfiguredServices,
    PostConfiguredServices,
    ServicesFrozen,
    Initialized,
    Failed,
    Shutdown
}
