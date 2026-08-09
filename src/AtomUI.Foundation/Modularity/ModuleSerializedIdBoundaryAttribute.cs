namespace AtomUI.Modularity;

[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Method |
    AttributeTargets.Constructor |
    AttributeTargets.Field |
    AttributeTargets.Property,
    Inherited = false)]
public sealed class ModuleSerializedIdBoundaryAttribute : Attribute
{
}
