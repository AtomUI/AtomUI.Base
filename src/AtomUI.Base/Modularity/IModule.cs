namespace AtomUI.Modularity;

public interface IModule
{
    ValueTask PreConfigureServicesAsync(
        ModuleServiceConfigurationContext context,
        CancellationToken cancellationToken = default);

    ValueTask ConfigureServicesAsync(
        ModuleServiceConfigurationContext context,
        CancellationToken cancellationToken = default);

    ValueTask PostConfigureServicesAsync(
        ModuleServiceConfigurationContext context,
        CancellationToken cancellationToken = default);

    ValueTask InitializeAsync(
        ModuleInitializationContext context,
        CancellationToken cancellationToken = default);

    ValueTask ShutdownAsync(
        ModuleShutdownContext context,
        CancellationToken cancellationToken = default);
}
