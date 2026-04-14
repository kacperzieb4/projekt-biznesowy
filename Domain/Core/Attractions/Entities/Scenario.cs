using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Entities;

/// <summary>
/// A specific variant/way of experiencing an attraction (e.g. "Full Tour" vs "Express Tour").
/// Each scenario has its own duration, tags, and availability schedule.
/// </summary>
public class Scenario
{
    public ScenarioId Id { get; }
    public string Name { get; }
    public TimeSpan Duration { get; }
    public List<Tag> Tags { get; }
    public AvailabilitySchedule Schedule { get; }

    public Scenario(ScenarioId id, string name, TimeSpan duration, List<Tag> tags, AvailabilitySchedule schedule)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(schedule);

        Id = id;
        Name = name;
        Duration = duration;
        Tags = tags ?? [];
        Schedule = schedule;
    }
}
