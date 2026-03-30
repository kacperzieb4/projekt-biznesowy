using TouristManagement.Domain.Core.Attractions;

namespace TouristManagement.Domain.Modules.CatalogSearch;

public record RuleId(Guid Value) { public static RuleId New() => new(Guid.NewGuid()); }
public record DateRange(DateTime Start, DateTime End);
public record GeoArea(double CenterLatitude, double CenterLongitude, double RadiusKm);
public enum RuleType { Weekly, Seasonal, Exception }
public enum Effect { Allow, Deny }

public class RuleDefinition
{
    public RuleId Id { get; init; }
    public RuleType Type { get; init; }
    public int Priority { get; init; }
    public Effect Effect { get; init; }
    public Dictionary<string, object> Params { get; init; } = new();
}

public class AvailabilitySchedule
{
    public int BasePriority { get; set; }
    public List<RuleDefinition> ActiveRules { get; } = new();

    public bool IsAvailable(DateTime time, List<RuleDefinition> inheritedRules = null)
    {
        var allRules = (inheritedRules ?? new List<RuleDefinition>())
            .Concat(ActiveRules)
            .OrderByDescending(r => r.Priority + BasePriority)
            .ToList();

        if (!allRules.Any()) return true;

        foreach (var rule in allRules)
        {
            // Przykład uproszczonej logiki reguł tygodniowych
            if (rule.Type == RuleType.Weekly && rule.Params.TryGetValue("DayOfWeek", out var dow))
                if (time.DayOfWeek.ToString() == dow.ToString()) return rule.Effect == Effect.Allow;
        }
        return true;
    }
}

public record SearchCriteria(
    DateRange TimeRange,
    GeoArea? NearArea = null,
    TimeSpan? RequiredDuration = null,
    List<TagId>? RequiredTags = null,
    List<TagId>? ExcludedTags = null
);

public record AvailabilityResult(
    AttractionId ComponentId, 
    bool IsAvailable, 
    List<ScenarioId> AvailableScenarios, 
    List<RuleId> AppliedRules
);