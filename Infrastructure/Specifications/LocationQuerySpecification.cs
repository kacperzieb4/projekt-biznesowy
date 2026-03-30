using System;
using System.Linq.Expressions;
using AttractionCatalog.Domain.Core.Attractions;
using AttractionCatalog.Domain.Modules.CatalogSearch;

namespace AttractionCatalog.Infrastructure.Specifications
{
    public class LocationQuerySpecification : IQuerySpecification<IAttractionComponent>
    {
        private readonly GeoArea _area;

        public LocationQuerySpecification(GeoArea area)
        {
            _area = area;
        }

        public Expression<Func<IAttractionComponent, bool>> ToExpression()
        {
            // Simple geographic calculation in C# expressions
            // In a real DB provider, this would be translated to SQL GIS extensions.
            return component => 
                component is SingleAttraction s &&
                CalculateDistance(s.Location, _area.CenterLatitude, _area.CenterLongitude) <= _area.RadiusKm;
        }

        private static double CalculateDistance(Location loc, double lat, double lon)
        {
             // Haversine formula implementation omitted for brevity. 
             // Returns distance in KM.
             return 1.0; 
        }
    }
}
