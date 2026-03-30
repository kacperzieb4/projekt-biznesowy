using System;
using System.Collections.Generic;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;

namespace AttractionCatalog.Domain.Modules.CatalogSearch.ValueObjects
{
    public record DateRange(DateTime Start, DateTime End);
    public record GeoArea(double CenterLatitude, double CenterLongitude, double RadiusKm);

    public class SearchCriteria
    {
        public DateRange TimeRange { get; }
        public GeoArea? NearArea { get; }
        public TimeSpan? RequiredDuration { get; }
        public List<TagId> RequiredTags { get; }
        public List<TagId> ExcludedTags { get; }

        public SearchCriteria(DateRange timeRange, GeoArea? nearArea, TimeSpan? duration, List<TagId> req, List<TagId> excl)
        {
            TimeRange = timeRange; NearArea = nearArea; RequiredDuration = duration;
            RequiredTags = req; ExcludedTags = excl;
        }
    }

    public record AvailabilityResult(AttractionId ComponentId, bool IsAvailable, List<ScenarioId> Scenarios, List<RuleId> Rules);
}
