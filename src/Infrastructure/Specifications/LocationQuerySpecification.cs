using Domain.Core.Attractions.Entities;
using Domain.Core.Attractions.Interfaces;
using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Specifications;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Infrastructure.Specifications;

/// <summary>
/// Query specification for pre-filtering attractions by geographic proximity.
/// 
/// Applied conditionally — ONLY when SearchCriteria.NearArea is provided.
/// Calculates distance between attraction's Location and the GeoArea center,
/// rejecting attractions outside the specified radius.
/// 
/// This runs at the repository level (conceptually at the database level)
/// to avoid loading all attractions into memory.
/// </summary>
public sealed class LocationQuerySpecification : IQuerySpecification<IAttractionComponent>
{
    private readonly GeoArea _searchArea;

    public LocationQuerySpecification(GeoArea searchArea)
    {
        _searchArea = searchArea ?? throw new ArgumentNullException(nameof(searchArea));
    }

    /// <summary>
    /// Produces a predicate that filters attraction components by geographic proximity.
    /// Only SingleAttraction entities have a Location; groups are always included
    /// (their availability is determined by child components).
    /// </summary>
    public Func<IAttractionComponent, bool> ToExpression()
    {
        return component =>
        {
            if (component is not SingleAttraction single)
                return true; // Groups don't have location — always pass geo filter

            var centerLocation = new Location(_searchArea.CenterLatitude, _searchArea.CenterLongitude);
            var distanceKm = single.Location.DistanceToKm(centerLocation);

            return distanceKm <= _searchArea.RadiusKm;
        };
    }
}
