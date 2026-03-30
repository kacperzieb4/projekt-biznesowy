namespace Domain.Core.Attractions.ValueObjects;

/// <summary>
/// Strongly-typed identifier for Tags.
/// </summary>
public sealed record TagId
{
    public Guid Value { get; }

    public TagId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("TagId cannot be empty.", nameof(value));

        Value = value;
    }

    public static TagId CreateNew() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
