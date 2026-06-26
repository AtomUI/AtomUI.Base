namespace AtomUI.Modularity;

public readonly record struct ModuleId
{
    private ModuleId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
    }

    public string Value { get; }

    public static ModuleId FromModuleType<TModule>()
        where TModule : IModule
    {
        return FromModuleType(typeof(TModule));
    }

    public static ModuleId FromModuleType(Type moduleType)
    {
        ArgumentNullException.ThrowIfNull(moduleType);

        if (!typeof(IModule).IsAssignableFrom(moduleType))
        {
            throw new ArgumentException("Module type must implement IModule.", nameof(moduleType));
        }

        return new ModuleId(ModuleTypeIdentity.FromType(moduleType));
    }

    public static ModuleId FromSerializedValue(string value)
    {
        return new ModuleId(value);
    }

    internal static ModuleId FromModuleTypeName(string assemblyName, string typeName)
    {
        return new ModuleId(ModuleTypeIdentity.FromTypeName(assemblyName, typeName));
    }

    public override string ToString()
    {
        return Value;
    }
}
