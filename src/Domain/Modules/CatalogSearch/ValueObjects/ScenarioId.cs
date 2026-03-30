namespace Domain.Modules.CatalogSearch.ValueObjects;

/// <summary>
/// Strongly-typed identifier for attraction scenarios (variants).
/// </summary>
public sealed record ScenarioId
{
    public Guid Value { get; }

    public ScenarioId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ScenarioId cannot be empty.", nameof(value));

        Value = value;
    }

    public static ScenarioId CreateNew() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
