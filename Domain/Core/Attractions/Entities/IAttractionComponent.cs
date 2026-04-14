using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Core.Attractions.Entities;

/// <summary>
/// Composite pattern interface — allows treating a single attraction
/// and an attraction group uniformly for availability checks and catalog search.
/// </summary>
public interface IAttractionComponent
{
    AttractionId Id { get; }
    string Name { get; }
    List<Tag> Tags { get; }
    AvailabilitySchedule Schedule { get; }
}
