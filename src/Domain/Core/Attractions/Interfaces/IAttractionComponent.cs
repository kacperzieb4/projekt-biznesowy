using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Entities;

namespace Domain.Core.Attractions.Interfaces;

/// <summary>
/// Composite pattern interface — unifies SingleAttraction and AttractionGroup
/// behind a common contract, allowing uniform treatment of individual
/// attractions and composite packages.
/// </summary>
public interface IAttractionComponent
{
    /// <summary>Unique identifier of the attraction component.</summary>
    AttractionId Id { get; }

    /// <summary>Display name of the attraction or group.</summary>
    string Name { get; }

    /// <summary>Tags associated with this component for categorization and search.</summary>
    IReadOnlyList<Tag> Tags { get; }

    /// <summary>Availability schedule governing when this component is accessible.</summary>
    AvailabilitySchedule Schedule { get; }
}
