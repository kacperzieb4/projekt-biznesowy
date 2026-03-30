using System.Collections.Generic;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Entities
{
    public class SingleAttraction : IAttractionComponent
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
            Id = id;
            Name = name;
            State = state;
            Tags = tags;
            Location = location;
            Schedule = schedule;
            Scenarios = scenarios;
        }

        public void Publish()
        {
            if (State == AttractionState.Draft)
            {
                State = AttractionState.Catalog;
            }
        }
    }
}
