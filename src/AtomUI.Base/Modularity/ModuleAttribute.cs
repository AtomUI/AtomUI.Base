namespace AtomUI.Modularity;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ModuleAttribute : Attribute
{
    public string? DisplayName { get; set; }

    public Type[] Dependencies { get; set; } = Array.Empty<Type>();
}
