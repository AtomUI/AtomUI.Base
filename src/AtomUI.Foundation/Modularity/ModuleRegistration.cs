namespace AtomUI.Modularity;

public sealed class ModuleRegistration
{
    private ModuleRegistration(ModuleDescriptor descriptor, Func<IModule> factory)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(factory);

        Descriptor = descriptor;
        Factory = factory;
    }

    public ModuleDescriptor Descriptor { get; }

    public Func<IModule> Factory { get; }

    public static ModuleRegistration For<TModule>(
        ModuleDescriptor descriptor,
        Func<TModule> factory)
        where TModule : IModule
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(factory);

        var expectedModuleId = ModuleId.FromModuleType<TModule>();
        if (descriptor.Id != expectedModuleId)
        {
            throw new ArgumentException(
                $"Descriptor module id '{descriptor.Id}' does not match module type '{expectedModuleId}'.",
                nameof(descriptor));
        }

        return new ModuleRegistration(descriptor, () => factory());
    }
}
