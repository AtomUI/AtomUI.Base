namespace AtomUI.Modularity;

public sealed class ModuleGraphException : Exception
{
    public ModuleGraphException(IReadOnlyList<ModuleDiagnostic> diagnostics)
        : base(CreateMessage(diagnostics))
    {
        ArgumentNullException.ThrowIfNull(diagnostics);

        Diagnostics = Array.AsReadOnly(diagnostics.ToArray());
    }

    public IReadOnlyList<ModuleDiagnostic> Diagnostics { get; }

    private static string CreateMessage(IReadOnlyList<ModuleDiagnostic> diagnostics)
    {
        if (diagnostics.Count == 0)
        {
            return "Module dependency graph failed.";
        }

        return diagnostics[0].Message;
    }
}
