namespace AtomUI.Modularity;

public sealed class ModuleServiceDescriptor
{
    public ModuleServiceDescriptor(
        Type serviceType,
        ModuleServiceLifetime lifetime,
        Type? implementationType = null,
        object? instance = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (!Enum.IsDefined(lifetime))
        {
            throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Module service lifetime is not supported.");
        }

        if ((implementationType is null) == (instance is null))
        {
            throw new ArgumentException(
                "A module service descriptor must provide exactly one implementation type or instance.");
        }

        if (implementationType is not null && !serviceType.IsAssignableFrom(implementationType))
        {
            throw new ArgumentException(
                $"Implementation type '{implementationType.FullName}' is not assignable to service type '{serviceType.FullName}'.",
                nameof(implementationType));
        }

        if (instance is not null && !serviceType.IsInstanceOfType(instance))
        {
            throw new ArgumentException(
                $"Instance type '{instance.GetType().FullName}' is not assignable to service type '{serviceType.FullName}'.",
                nameof(instance));
        }

        ServiceType = serviceType;
        Lifetime = lifetime;
        ImplementationType = implementationType;
        Instance = instance;
    }

    public Type ServiceType { get; }

    public ModuleServiceLifetime Lifetime { get; }

    public Type? ImplementationType { get; }

    public object? Instance { get; }
}
