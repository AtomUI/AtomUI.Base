namespace AtomUI.Modularity;

public sealed class ModuleDescriptor
{
    private readonly IReadOnlyList<ModuleDependencyDescriptor> _dependencies;

    internal ModuleDescriptor(
        ModuleId id,
        string displayName,
        IEnumerable<ModuleDependencyDescriptor>? dependencies = null,
        string? typeName = null,
        string? sourceAssemblyName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        Id = id;
        DisplayName = displayName;
        TypeName = typeName;
        SourceAssemblyName = sourceAssemblyName;
        _dependencies = Array.AsReadOnly((dependencies ?? []).ToArray());
    }

    public ModuleId Id { get; }

    public string DisplayName { get; }

    public string? TypeName { get; }

    public string? SourceAssemblyName { get; }

    public IReadOnlyList<ModuleDependencyDescriptor> Dependencies => _dependencies;

    public static ModuleDescriptor For<TModule>(
        string displayName,
        IEnumerable<ModuleDependencyDescriptor>? dependencies = null)
        where TModule : IModule
    {
        return CreateForModule<TModule>(
            displayName,
            dependencies);
    }

    private static ModuleDescriptor CreateForModule<TModule>(
        string displayName,
        IEnumerable<ModuleDependencyDescriptor>? dependencies)
        where TModule : IModule
    {
        var moduleType = typeof(TModule);
        return new ModuleDescriptor(
            ModuleId.FromModuleType(moduleType),
            displayName,
            dependencies,
            moduleType.FullName,
            moduleType.Assembly.GetName().Name);
    }
}
