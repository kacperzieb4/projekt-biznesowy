using System.Collections.Generic;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Aggregates
{
    public interface IAttractionComponent
    {
        ValueObjects.AttractionId Id { get; }
        string Name { get; }
        List<Tag> Tags { get; }
        AvailabilitySchedule Schedule { get; }
    }
}
