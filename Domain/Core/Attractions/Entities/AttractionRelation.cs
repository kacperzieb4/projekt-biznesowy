using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;

namespace AttractionCatalog.Domain.Core.Attractions.Entities;

/// <summary>
/// Directed relationship between two attractions (e.g. "Wawel requires Museum ticket").
/// Stored as a separate entity to enable validation (e.g. cycle detection) and metadata.
/// </summary>
public class AttractionRelation
{
    public AttractionId From { get; }
    public AttractionId To { get; }
    public RelationType Type { get; }

    public AttractionRelation(AttractionId from, AttractionId to, RelationType type)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        From = from;
        To = to;
        Type = type;
    }
}
