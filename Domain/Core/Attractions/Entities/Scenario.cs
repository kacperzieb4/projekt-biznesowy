using System;
using System.Collections.Generic;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Entities
{
    public class Scenario
    {
        public ScenarioId Id { get; }
        public string Name { get; }
        public TimeSpan Duration { get; }
        public List<Tag> Tags { get; }
        public AvailabilitySchedule Schedule { get; }

        public Scenario(ScenarioId id, string name, TimeSpan duration, List<Tag> tags, AvailabilitySchedule schedule)
        {
            Id = id;
            Name = name;
            Duration = duration;
            Tags = tags;
            Schedule = schedule;
        }
    }
}
