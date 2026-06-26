namespace AtomUI.Modularity;

public sealed class ModulePhaseContext
{
    internal ModulePhaseContext(
        ModuleDescriptor descriptor,
        ModuleServiceRegistry services)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(services);

        Descriptor = descriptor;
        Services = services;
    }

    public ModuleDescriptor Descriptor { get; }

    public ModuleServiceRegistry Services { get; }
}
