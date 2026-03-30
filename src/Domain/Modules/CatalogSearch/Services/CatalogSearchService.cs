using Domain.Core.Attractions.Aggregates;
using Domain.Core.Attractions.Entities;
using Domain.Core.Attractions.Interfaces;
using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Entities;
using Domain.Modules.CatalogSearch.Specifications;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Domain.Modules.CatalogSearch.Services;

/// <summary>
/// Domain Service responsible for evaluating attraction availability
/// against SearchCriteria. Orchestrates rule compilation, tag filtering,
/// duration filtering, and composite (group) logic.
/// 
/// This is a stateless service — does not hold any mutable state.
/// Receives pre-filtered data from the Application layer (already filtered by IQuerySpec).
/// 
/// Evaluation algorithm for groups:
/// 1. Evaluate own schedule rules  
/// 2. Recursively check all children
/// 3. Group is available only if own rules ALLOW AND every child has ≥1 available scenario
/// </summary>
public sealed class CatalogSearchService
{
    private readonly RuleSpecificationCompiler _ruleCompiler;

    public CatalogSearchService(RuleSpecificationCompiler ruleCompiler)
    {
        _ruleCompiler = ruleCompiler ?? throw new ArgumentNullException(nameof(ruleCompiler));
    }

    /// <summary>
    /// Evaluates availability for a list of pre-filtered attraction components.
    /// Returns availability results with scenario-level detail.
    /// </summary>
    public IReadOnlyList<AvailabilityResult> FindAvailableAttractions(
        SearchCriteria criteria,
        IReadOnlyList<IAttractionComponent> preFiltered)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ArgumentNullException.ThrowIfNull(preFiltered);

        var results = new List<AvailabilityResult>();

        foreach (var component in preFiltered)
        {
            var result = EvaluateComponent(component, criteria);
            if (result is not null)
                results.Add(result);
        }

        return results.AsReadOnly();
    }

    // ── Evaluation Logic ───────────────────────────────────────────────

    private AvailabilityResult? EvaluateComponent(IAttractionComponent component, SearchCriteria criteria)
    {
        // Apply tag filters first (fast reject)
        if (!PassesTagFilter(component, criteria))
            return null;

        return component switch
        {
            SingleAttraction single => EvaluateSingleAttraction(single, criteria),
            AttractionGroup group => EvaluateGroup(group, criteria),
            _ => null
        };
    }

    private AvailabilityResult EvaluateSingleAttraction(SingleAttraction attraction, SearchCriteria criteria)
    {
        // 1. Check global availability of the attraction
        var globalSpec = _ruleCompiler.Compile(attraction.Schedule.GetActiveRulesOrdered());
        var globalAvailable = globalSpec.IsSatisfiedBy(criteria);

        if (!globalAvailable)
        {
            return new AvailabilityResult(
                attraction.Id,
                isAvailable: false,
                appliedRules: attraction.Schedule.ActiveRuleIds);
        }

        // 2. Check each scenario — intersection logic (Global AND Scenario must be ALLOW)
        var availableScenarioIds = new List<ScenarioId>();
        var allAppliedRules = new List<RuleId>(attraction.Schedule.ActiveRuleIds);

        foreach (var scenario in attraction.Scenarios)
        {
            if (PassesScenarioFilters(scenario, criteria))
            {
                var scenarioSpec = _ruleCompiler.Compile(scenario.Schedule.GetActiveRulesOrdered());
                if (scenarioSpec.IsSatisfiedBy(criteria))
                {
                    availableScenarioIds.Add(scenario.Id);
                }
                allAppliedRules.AddRange(scenario.Schedule.ActiveRuleIds);
            }
        }

        return new AvailabilityResult(
            attraction.Id,
            isAvailable: availableScenarioIds.Count > 0,
            availableScenarios: availableScenarioIds,
            appliedRules: allAppliedRules);
    }

    private AvailabilityResult EvaluateGroup(AttractionGroup group, SearchCriteria criteria)
    {
        // 1. Evaluate group's own schedule
        var groupSpec = _ruleCompiler.Compile(group.Schedule.GetActiveRulesOrdered());
        if (!groupSpec.IsSatisfiedBy(criteria))
        {
            return new AvailabilityResult(
                group.Id,
                isAvailable: false,
                appliedRules: group.Schedule.ActiveRuleIds);
        }

        // 2. Recursively evaluate all children — ALL must have at least one available scenario
        var allAppliedRules = new List<RuleId>(group.Schedule.ActiveRuleIds);
        var allChildrenAvailable = true;

        foreach (var child in group.Components)
        {
            var childResult = EvaluateComponent(child, criteria);
            if (childResult is null || !childResult.IsAvailable)
            {
                allChildrenAvailable = false;
                break;
            }
            allAppliedRules.AddRange(childResult.AppliedRules);
        }

        return new AvailabilityResult(
            group.Id,
            isAvailable: allChildrenAvailable,
            appliedRules: allAppliedRules);
    }

    // ── Filter Helpers ─────────────────────────────────────────────────

    private static bool PassesTagFilter(IAttractionComponent component, SearchCriteria criteria)
    {
        if (!criteria.HasTagFilters)
            return true;

        var componentTagIds = component.Tags.Select(t => t.Id).ToHashSet();

        // All required tags must be present
        if (criteria.RequiredTags.Any(rt => !componentTagIds.Contains(rt)))
            return false;

        // None of the excluded tags may be present
        if (criteria.ExcludedTags.Any(et => componentTagIds.Contains(et)))
            return false;

        return true;
    }

    private static bool PassesScenarioFilters(Scenario scenario, SearchCriteria criteria)
    {
        // Duration filter
        if (criteria.HasDurationFilter && scenario.Duration < criteria.RequiredDuration!.Value)
            return false;

        // Scenario-level tag filter (additional tags on scenario level)
        if (criteria.HasTagFilters)
        {
            var scenarioTagIds = scenario.Tags.Select(t => t.Id).ToHashSet();
            if (criteria.ExcludedTags.Any(et => scenarioTagIds.Contains(et)))
                return false;
        }

        return true;
    }
}
