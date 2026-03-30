using System.Collections.Generic;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Aggregates
{
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
            Id = id; Name = name; SequenceMode = sequenceMode; Tags = tags; Schedule = schedule; Components = components;
        }
    }
}
