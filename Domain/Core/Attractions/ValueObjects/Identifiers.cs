namespace AttractionCatalog.Domain.Core.Attractions.ValueObjects;

// Strong-typed identifiers prevent accidental mix-ups between different entity IDs.
public record AttractionId(Guid Value);
public record TagId(Guid Value);
public record ScenarioId(Guid Value);
public record RuleId(Guid Value);

/// <summary>
/// Geographic point on the map. Immutable value object assigned to a SingleAttraction.
/// </summary>
public record Location(double Latitude, double Longitude)
{
    /// <summary>
    /// Calculates the great-circle distance to another point using the Haversine formula.
    /// </summary>
    public double DistanceToKm(Location other)
    {
        const double earthRadiusKm = 6371.0;
        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
