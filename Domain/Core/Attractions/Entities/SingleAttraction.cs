using AttractionCatalog.Domain.Common.Models;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Entities;

/// <summary>
/// Domain event raised when an attraction transitions from Draft to Catalog.
/// </summary>
public record AttractionPublishedEvent(AttractionId Id) : DomainEvent;

/// <summary>
/// Aggregate root representing a single bookable attraction (museum, tour, event, etc.).
/// Owns its lifecycle (Draft → Catalog → Archived) and contains Scenarios as child entities.
/// </summary>
public class SingleAttraction : AggregateRoot, IAttractionComponent
{
    public AttractionId Id { get; }
    public string Name { get; }
    public AttractionState State { get; private set; }
    public List<Tag> Tags { get; }
    public Location Location { get; }
    public AvailabilitySchedule Schedule { get; }
    public List<Scenario> Scenarios { get; }

    public SingleAttraction(
        AttractionId id,
        string name,
        AttractionState state,
        List<Tag> tags,
        Location location,
        AvailabilitySchedule schedule,
        List<Scenario> scenarios)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(location);
        ArgumentNullException.ThrowIfNull(schedule);

        Id = id;
        Name = name;
        State = state;
        Tags = tags ?? [];
        Location = location;
        Schedule = schedule;
        Scenarios = scenarios ?? [];
    }

    /// <summary>
    /// Transitions the attraction from Draft to Catalog, making it publicly visible.
    /// Raises <see cref="AttractionPublishedEvent"/> upon success.
    /// </summary>
    public void Publish()
    {
        if (State != AttractionState.Draft)
            return;

        State = AttractionState.Catalog;
        AddEvent(new AttractionPublishedEvent(Id));
    }
}
