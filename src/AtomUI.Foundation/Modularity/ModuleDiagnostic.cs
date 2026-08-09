using System.Collections.ObjectModel;

namespace AtomUI.Modularity;

public sealed record ModuleDiagnostic
{
    public ModuleDiagnostic(
        string code,
        string message,
        ModuleId? moduleId = null,
        ModuleId? relatedModuleId = null,
        ModuleDiagnosticSeverity severity = ModuleDiagnosticSeverity.Error,
        string? stage = null,
        string? sourceAssemblyName = null,
        string? exceptionType = null,
        IReadOnlyList<ModuleId>? dependencyPath = null,
        IReadOnlyDictionary<string, string>? context = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
        ModuleId = moduleId;
        RelatedModuleId = relatedModuleId;
        Severity = severity;
        Stage = stage;
        SourceAssemblyName = sourceAssemblyName;
        ExceptionType = exceptionType;
        DependencyPath = dependencyPath is null
            ? null
            : Array.AsReadOnly(dependencyPath.ToArray());
        Context = context is null
            ? null
            : new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(context));
    }

    public string Code { get; }

    public string Message { get; }

    public ModuleId? ModuleId { get; }

    public ModuleId? RelatedModuleId { get; }

    public ModuleDiagnosticSeverity Severity { get; }

    public string? Stage { get; }

    public string? SourceAssemblyName { get; }

    public string? ExceptionType { get; }

    public IReadOnlyList<ModuleId>? DependencyPath { get; }

    public IReadOnlyDictionary<string, string>? Context { get; }
}
