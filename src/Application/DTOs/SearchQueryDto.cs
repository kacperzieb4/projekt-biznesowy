namespace Application.DTOs;

/// <summary>
/// Request DTO for catalog search queries.
/// Maps API-level input to Application layer processing.
/// </summary>
public sealed record SearchQueryDto
{
    /// <summary>Start of the time range for availability.</summary>
    public required DateTime StartDate { get; init; }

    /// <summary>End of the time range for availability.</summary>
    public required DateTime EndDate { get; init; }

    /// <summary>Optional: center latitude for geographic filtering.</summary>
    public double? CenterLatitude { get; init; }

    /// <summary>Optional: center longitude for geographic filtering.</summary>
    public double? CenterLongitude { get; init; }

    /// <summary>Optional: radius in km for geographic filtering.</summary>
    public double? RadiusKm { get; init; }

    /// <summary>Optional: minimum required duration in minutes.</summary>
    public int? RequiredDurationMinutes { get; init; }

    /// <summary>Tag IDs that must be present on results.</summary>
    public List<Guid> RequiredTagIds { get; init; } = new();

    /// <summary>Tag IDs that must NOT be present on results.</summary>
    public List<Guid> ExcludedTagIds { get; init; } = new();

    /// <summary>Whether geographic filter is present and valid.</summary>
    public bool HasGeoFilter => CenterLatitude.HasValue && CenterLongitude.HasValue && RadiusKm.HasValue;
}
