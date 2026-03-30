namespace Domain.Core.Attractions.ValueObjects;

/// <summary>
/// Strongly-typed identifier for attractions (both SingleAttraction and AttractionGroup).
/// Prevents primitive obsession and compile-time type safety against mixing IDs of different entities.
/// </summary>
public sealed record AttractionId
{
    public Guid Value { get; }

    public AttractionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AttractionId cannot be empty.", nameof(value));

        Value = value;
    }

    public static AttractionId CreateNew() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
