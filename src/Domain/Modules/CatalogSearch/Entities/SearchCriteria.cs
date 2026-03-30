using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Domain.Modules.CatalogSearch.Entities;

/// <summary>
/// Value Object encapsulating all search/filter parameters for catalog queries.
/// Combines temporal, geographical, and tag-based predicates.
/// 
/// Design note: SearchCriteria is part of the domain (not application layer)
/// because the domain service CatalogSearchService operates on it directly.
/// </summary>
public sealed class SearchCriteria
{
    /// <summary>Time range for availability check.</summary>
    public DateRange TimeRange { get; }

    /// <summary>Optional geographic area filter (when null, no spatial filtering).</summary>
    public GeoArea? NearArea { get; }

    /// <summary>Optional minimum required duration filter.</summary>
    public TimeSpan? RequiredDuration { get; }

    /// <summary>Tags that must be present on the attraction (inclusive/AND logic).</summary>
    public IReadOnlyList<TagId> RequiredTags { get; }

    /// <summary>Tags that must NOT be present on the attraction (exclusive filter).</summary>
    public IReadOnlyList<TagId> ExcludedTags { get; }

    public SearchCriteria(
        DateRange timeRange,
        GeoArea? nearArea = null,
        TimeSpan? requiredDuration = null,
        IEnumerable<TagId>? requiredTags = null,
        IEnumerable<TagId>? excludedTags = null)
    {
        TimeRange = timeRange ?? throw new ArgumentNullException(nameof(timeRange));
        NearArea = nearArea;
        RequiredDuration = requiredDuration;
        RequiredTags = requiredTags?.ToList().AsReadOnly()
            ?? new List<TagId>().AsReadOnly();
        ExcludedTags = excludedTags?.ToList().AsReadOnly()
            ?? new List<TagId>().AsReadOnly();
    }

    /// <summary>Whether spatial pre-filtering should be applied.</summary>
    public bool HasGeoFilter => NearArea is not null;

    /// <summary>Whether duration-based filtering should be applied.</summary>
    public bool HasDurationFilter => RequiredDuration.HasValue;

    /// <summary>Whether tag-based filtering should be applied.</summary>
    public bool HasTagFilters => RequiredTags.Count > 0 || ExcludedTags.Count > 0;
}
