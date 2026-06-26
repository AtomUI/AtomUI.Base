namespace AtomUI.Modularity;

public sealed class ModuleServiceConfigurationContext
{
    internal ModuleServiceConfigurationContext(ModuleDescriptor descriptor, ModuleServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(services);

        Descriptor = descriptor;
        Services = services;
    }

    public ModuleDescriptor Descriptor { get; }

    public ModuleServiceCollection Services { get; }
}
