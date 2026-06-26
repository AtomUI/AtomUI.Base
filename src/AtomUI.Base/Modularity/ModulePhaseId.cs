namespace AtomUI.Modularity;

public readonly record struct ModulePhaseId
{
    private ModulePhaseId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
    }

    public string Value { get; }

    public static ModulePhaseId FromType<TPhase>()
    {
        return FromType(typeof(TPhase));
    }

    public static ModulePhaseId FromType(Type phaseType)
    {
        return new ModulePhaseId(ModuleTypeIdentity.FromType(phaseType));
    }

    public static ModulePhaseId FromSerializedValue(string value)
    {
        return new ModulePhaseId(value);
    }

    public override string ToString()
    {
        return Value;
    }
}
