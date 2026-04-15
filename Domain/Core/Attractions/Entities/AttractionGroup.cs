using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Entities;

/// <summary>
/// A composite product made of multiple attractions (e.g. "Kraków Pass").
/// Groups can contain other groups, forming a recursive tree structure.
/// </summary>
public class AttractionGroup : IAttractionComponent
{
    public AttractionId Id { get; }
    public string Name { get; }
    public SequenceMode SequenceMode { get; }
    public List<Tag> Tags { get; }
    public AvailabilitySchedule Schedule { get; }
    public List<IAttractionComponent> Components { get; }

    public AttractionGroup(
        AttractionId id,
        string name,
        SequenceMode sequenceMode,
        List<Tag> tags,
        AvailabilitySchedule schedule,
        List<IAttractionComponent> components)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(schedule);

        Id = id;
        Name = name;
        SequenceMode = sequenceMode;
        Tags = tags ?? [];
        Schedule = schedule;
        Components = components ?? [];
    }
}
