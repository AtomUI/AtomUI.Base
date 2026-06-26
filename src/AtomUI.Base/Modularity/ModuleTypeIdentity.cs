namespace AtomUI.Modularity;

internal static class ModuleTypeIdentity
{
    public static string FromType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return FromTypeName(
            type.Assembly.GetName().Name ?? string.Empty,
            type.FullName ?? type.Name);
    }

    public static string FromTypeName(string assemblyName, string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);

        return $"{assemblyName}/{typeName}";
    }
}
