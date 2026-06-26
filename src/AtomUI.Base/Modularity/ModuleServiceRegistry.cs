namespace AtomUI.Modularity;

public sealed class ModuleServiceRegistry
{
    private readonly ModuleServiceCollection _services;

    internal ModuleServiceRegistry(ModuleServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _services = services;
    }

    public bool IsFrozen => _services.IsFrozen;

    public IReadOnlyList<ModuleServiceDescriptor> Descriptors => _services.Descriptors;

    public TService GetRequiredService<TService>()
        where TService : notnull
    {
        return _services.GetRequiredService<TService>();
    }
}
