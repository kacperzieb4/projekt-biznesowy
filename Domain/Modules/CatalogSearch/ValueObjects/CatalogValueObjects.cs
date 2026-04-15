using AttractionCatalog.Domain.Core.Attractions.ValueObjects;

namespace AttractionCatalog.Domain.Modules.CatalogSearch.ValueObjects;

/// <summary>Inclusive date range for availability queries.</summary>
public record DateRange(DateTime Start, DateTime End);

/// <summary>Circular geographic area used for proximity-based search.</summary>
public record GeoArea(double CenterLatitude, double CenterLongitude, double RadiusKm);

/// <summary>
/// Encapsulates all search criteria for the catalog search use case.
/// Combines time range, geographic area, duration, and tag-based filtering.
/// </summary>
public class SearchCriteria
{
    public DateRange TimeRange { get; }
    public GeoArea? NearArea { get; }
    public TimeSpan? RequiredDuration { get; }
    public List<TagId> RequiredTags { get; }
    public List<TagId> ExcludedTags { get; }

    public SearchCriteria(
        DateRange timeRange,
        GeoArea? nearArea,
        TimeSpan? duration,
        List<TagId> requiredTags,
        List<TagId> excludedTags)
    {
        ArgumentNullException.ThrowIfNull(timeRange);

        TimeRange = timeRange;
        NearArea = nearArea;
        RequiredDuration = duration;
        RequiredTags = requiredTags ?? [];
        ExcludedTags = excludedTags ?? [];
    }
}

/// <summary>
/// Result of an availability check for a single component.
/// </summary>
public record AvailabilityResult(
    AttractionId ComponentId,
    bool IsAvailable,
    List<ScenarioId> Scenarios,
    List<RuleId> Rules);
