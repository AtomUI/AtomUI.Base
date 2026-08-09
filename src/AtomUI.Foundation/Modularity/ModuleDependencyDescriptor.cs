namespace AtomUI.Modularity;

public sealed class ModuleDependencyDescriptor
{
    internal ModuleDependencyDescriptor(ModuleId moduleId)
    {
        ModuleId = moduleId;
    }

    public ModuleId ModuleId { get; }

    internal static ModuleDependencyDescriptor Required(ModuleId moduleId)
    {
        return new ModuleDependencyDescriptor(moduleId);
    }

    public static ModuleDependencyDescriptor Required<TModule>()
        where TModule : IModule
    {
        return Required(ModuleId.FromModuleType<TModule>());
    }

    public static ModuleDependencyDescriptor Required(Type moduleType)
    {
        return Required(ModuleId.FromModuleType(moduleType));
    }
}
