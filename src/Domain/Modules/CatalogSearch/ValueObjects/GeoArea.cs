namespace Domain.Modules.CatalogSearch.ValueObjects;

/// <summary>
/// Value Object representing a geographic search area (circle on map).
/// Used exclusively in search queries (SearchCriteria), NOT stored with attraction entities.
/// </summary>
public sealed record GeoArea
{
    public double CenterLatitude { get; }
    public double CenterLongitude { get; }
    public double RadiusKm { get; }

    public GeoArea(double centerLatitude, double centerLongitude, double radiusKm)
    {
        if (centerLatitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(centerLatitude), "Latitude must be between -90 and 90.");

        if (centerLongitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(centerLongitude), "Longitude must be between -180 and 180.");

        if (radiusKm <= 0)
            throw new ArgumentOutOfRangeException(nameof(radiusKm), "Radius must be positive.");

        CenterLatitude = centerLatitude;
        CenterLongitude = centerLongitude;
        RadiusKm = radiusKm;
    }
}
