using System.Collections.Generic;
using AttractionCatalog.Domain.Common.Models;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Aggregates
{
    public record AttractionPublishedEvent(AttractionId Id) : DomainEvent;

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
            AttractionId id, string name, AttractionState state, List<Tag> tags, 
            Location location, AvailabilitySchedule schedule, List<Scenario> scenarios)
        {
            Guard.AgainstNull(id, nameof(id));
            Guard.AgainstEmptyString(name, nameof(name));
            Guard.AgainstNull(location, nameof(location));
            Guard.AgainstNull(schedule, nameof(schedule));

            Id = id; Name = name; State = state; Tags = tags ?? new(); 
            Location = location; Schedule = schedule; Scenarios = scenarios ?? new();
        }

        public void Publish()
        {
            if (State == AttractionState.Draft)
            {
                State = AttractionState.Catalog;
                AddEvent(new AttractionPublishedEvent(Id));
            }
        }
    }
}
