using System.Collections.ObjectModel;

namespace AtomUI.Modularity;

public sealed class ModuleServiceCollection
{
    private readonly Dictionary<Type, object> _services = [];
    private readonly List<ModuleServiceDescriptor> _descriptors = [];
    private readonly ReadOnlyCollection<ModuleServiceDescriptor> _descriptorView;

    public ModuleServiceCollection()
    {
        _descriptorView = _descriptors.AsReadOnly();
    }

    public bool IsFrozen { get; private set; }

    public IReadOnlyList<ModuleServiceDescriptor> Descriptors => _descriptorView;

    public void AddSingleton<TService>(TService service)
        where TService : notnull
    {
        ThrowIfFrozen();
        ArgumentNullException.ThrowIfNull(service);
        ReplaceDescriptor(new ModuleServiceDescriptor(
            typeof(TService),
            ModuleServiceLifetime.Singleton,
            instance: service));
        _services[typeof(TService)] = service;
    }

    public void AddSingleton<TService, TImplementation>()
        where TService : notnull
        where TImplementation : TService
    {
        AddTypeDescriptor<TService, TImplementation>(ModuleServiceLifetime.Singleton);
    }

    public void AddScoped<TService, TImplementation>()
        where TService : notnull
        where TImplementation : TService
    {
        AddTypeDescriptor<TService, TImplementation>(ModuleServiceLifetime.Scoped);
    }

    public void AddTransient<TService, TImplementation>()
        where TService : notnull
        where TImplementation : TService
    {
        AddTypeDescriptor<TService, TImplementation>(ModuleServiceLifetime.Transient);
    }

    public void ReplaceSingleton<TService>(TService service)
        where TService : notnull
    {
        ThrowIfFrozen();
        RemoveCore(typeof(TService));
        AddSingleton(service);
    }

    public bool Remove<TService>()
        where TService : notnull
    {
        ThrowIfFrozen();
        return RemoveCore(typeof(TService));
    }

    public void Clear()
    {
        ThrowIfFrozen();
        _services.Clear();
        _descriptors.Clear();
    }

    public TService GetRequiredService<TService>()
        where TService : notnull
    {
        if (_services.TryGetValue(typeof(TService), out var service))
        {
            return (TService)service;
        }

        throw new InvalidOperationException($"Service '{typeof(TService).FullName}' is not registered.");
    }

    public void Freeze()
    {
        IsFrozen = true;
    }

    private void ReplaceDescriptor(ModuleServiceDescriptor descriptor)
    {
        RemoveCore(descriptor.ServiceType);
        _descriptors.Add(descriptor);
    }

    private void AddTypeDescriptor<TService, TImplementation>(ModuleServiceLifetime lifetime)
        where TService : notnull
        where TImplementation : TService
    {
        ThrowIfFrozen();
        ReplaceDescriptor(new ModuleServiceDescriptor(
            typeof(TService),
            lifetime,
            typeof(TImplementation)));
    }

    private bool RemoveCore(Type serviceType)
    {
        var removed = _services.Remove(serviceType);
        for (var index = _descriptors.Count - 1; index >= 0; index--)
        {
            if (_descriptors[index].ServiceType == serviceType)
            {
                _descriptors.RemoveAt(index);
                removed = true;
            }
        }

        return removed;
    }

    private void ThrowIfFrozen()
    {
        if (IsFrozen)
        {
            throw new InvalidOperationException("Module service collection is frozen.");
        }
    }
}
