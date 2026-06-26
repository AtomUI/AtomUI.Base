namespace AtomUI.Modularity;

public sealed class ModuleHostValidationResult
{
    private readonly IReadOnlyList<ModuleDiagnostic> _diagnostics;

    private ModuleHostValidationResult(IEnumerable<ModuleDiagnostic>? diagnostics = null)
    {
        _diagnostics = Array.AsReadOnly((diagnostics ?? []).ToArray());
    }

    public static ModuleHostValidationResult Success { get; } = new();

    public bool IsSuccess => _diagnostics.Count == 0;

    public IReadOnlyList<ModuleDiagnostic> Diagnostics => _diagnostics;

    public static ModuleHostValidationResult Fail(params ModuleDiagnostic[] diagnostics)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);
        return new ModuleHostValidationResult(diagnostics);
    }
}
