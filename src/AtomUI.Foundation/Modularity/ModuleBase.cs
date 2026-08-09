namespace AtomUI.Modularity;

public abstract class ModuleBase : IModule
{
    public virtual ValueTask PreConfigureServicesAsync(
        ModuleServiceConfigurationContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        PreConfigureServices(context);
        return ValueTask.CompletedTask;
    }

    public virtual void PreConfigureServices(ModuleServiceConfigurationContext context)
    {
    }

    public virtual ValueTask ConfigureServicesAsync(
        ModuleServiceConfigurationContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        ConfigureServices(context);
        return ValueTask.CompletedTask;
    }

    public virtual void ConfigureServices(ModuleServiceConfigurationContext context)
    {
    }

    public virtual ValueTask PostConfigureServicesAsync(
        ModuleServiceConfigurationContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        PostConfigureServices(context);
        return ValueTask.CompletedTask;
    }

    public virtual void PostConfigureServices(ModuleServiceConfigurationContext context)
    {
    }

    public virtual ValueTask InitializeAsync(
        ModuleInitializationContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        Initialize(context);
        return ValueTask.CompletedTask;
    }

    public virtual void Initialize(ModuleInitializationContext context)
    {
    }

    public virtual ValueTask ShutdownAsync(
        ModuleShutdownContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        Shutdown(context);
        return ValueTask.CompletedTask;
    }

    public virtual void Shutdown(ModuleShutdownContext context)
    {
    }
}
