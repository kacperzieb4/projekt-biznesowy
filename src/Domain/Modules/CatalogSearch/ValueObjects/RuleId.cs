namespace Domain.Modules.CatalogSearch.ValueObjects;

/// <summary>
/// Strongly-typed identifier for availability rules.
/// </summary>
public sealed record RuleId
{
    public Guid Value { get; }

    public RuleId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("RuleId cannot be empty.", nameof(value));

        Value = value;
    }

    public static RuleId CreateNew() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
