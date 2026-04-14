using System.Linq.Expressions;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.ValueObjects;

namespace AttractionCatalog.Application.Catalog.Specifications;

/// <summary>
/// Returns all attractions without filtering.
/// </summary>
public sealed class AllAttractionsSpecification : IQuerySpecification<IAttractionComponent>
{
    public Expression<Func<IAttractionComponent, bool>> ToExpression() => _ => true;
}

/// <summary>
/// Filters attractions by geographic proximity to a center point.
/// Only matches <see cref="SingleAttraction"/> within the specified radius.
/// </summary>
public sealed class LocationQuerySpecification : IQuerySpecification<IAttractionComponent>
{
    private readonly GeoArea _area;

    public LocationQuerySpecification(GeoArea area)
    {
        ArgumentNullException.ThrowIfNull(area);
        _area = area;
    }

    public Expression<Func<IAttractionComponent, bool>> ToExpression()
    {
        // Expression trees don't support pattern matching ('is' with variable),
        // so we use a cast after a type check.
        var center = new Location(_area.CenterLatitude, _area.CenterLongitude);
        var radiusKm = _area.RadiusKm;

        return component =>
            component is SingleAttraction &&
            ((SingleAttraction)component).Location.DistanceToKm(center) <= radiusKm;
    }
}
