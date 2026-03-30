using Domain.Core.Attractions.Enums;
using Domain.Core.Attractions.ValueObjects;

namespace Domain.Core.Attractions.Entities;

/// <summary>
/// Entity modeling relationships between attractions.
/// Separate from attraction classes to allow independent validation (e.g., cycle detection)
/// and metadata attachment without modifying attraction entities (Open/Closed Principle).
/// </summary>
public sealed class AttractionRelation
{
    /// <summary>Source attraction in the relationship.</summary>
    public AttractionId From { get; }

    /// <summary>Target attraction in the relationship.</summary>
    public AttractionId To { get; }

    /// <summary>Type of relationship: Requires, Excludes, or RecommendedAfter.</summary>
    public RelationType Type { get; }

    public AttractionRelation(AttractionId from, AttractionId to, RelationType type)
    {
        From = from ?? throw new ArgumentNullException(nameof(from));
        To = to ?? throw new ArgumentNullException(nameof(to));

        if (from == to)
            throw new ArgumentException("An attraction cannot have a relation to itself.");

        Type = type;
    }

    /// <summary>
    /// Factory method creating a new relation between two attractions.
    /// </summary>
    public static AttractionRelation Create(AttractionId from, AttractionId to, RelationType type)
        => new(from, to, type);
}
