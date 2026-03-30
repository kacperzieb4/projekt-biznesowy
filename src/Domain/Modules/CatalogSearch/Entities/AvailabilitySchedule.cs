using Domain.Modules.CatalogSearch.Enums;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Domain.Modules.CatalogSearch.Entities;

/// <summary>
/// Aggregate managing a set of availability rules for an attraction component.
/// Implements priority-based rule overriding:
///   EffectivePriority = BasePriority + Rule.Priority
/// 
/// The rule with the highest effective priority determines the final outcome.
/// This enables specific exceptions to override general rules
/// (e.g., a holiday closure overrides regular weekly hours).
/// </summary>
public sealed class AvailabilitySchedule
{
    private readonly List<RuleDefinition> _rules = new();

    /// <summary>
    /// Base priority for rules in this schedule level.
    /// Higher-level schedules (e.g., Scenario) should have higher base priorities
    /// than lower-level ones (e.g., Attraction), enabling hierarchical overrides.
    /// </summary>
    public int BasePriority { get; }

    /// <summary>IDs of currently active rules (subset of all rules).</summary>
    public IReadOnlyList<RuleId> ActiveRuleIds { get; private set; }

    /// <summary>All rule definitions associated with this schedule.</summary>
    public IReadOnlyList<RuleDefinition> Rules => _rules.AsReadOnly();

    public AvailabilitySchedule(int basePriority)
    {
        BasePriority = basePriority;
        ActiveRuleIds = new List<RuleId>();
    }

    /// <summary>
    /// Adds a rule to this schedule and activates it.
    /// </summary>
    public void AddRule(RuleDefinition rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (_rules.Any(r => r.Id == rule.Id))
            throw new InvalidOperationException($"Rule {rule.Id} already exists in this schedule.");

        _rules.Add(rule);
        ActivateRule(rule.Id);
    }

    /// <summary>
    /// Activates a rule by its ID (makes it participate in availability evaluation).
    /// </summary>
    public void ActivateRule(RuleId ruleId)
    {
        if (!_rules.Any(r => r.Id == ruleId))
            throw new InvalidOperationException($"Rule {ruleId} not found in this schedule.");

        var activeList = ActiveRuleIds.ToList();
        if (!activeList.Any(id => id == ruleId))
        {
            activeList.Add(ruleId);
            ActiveRuleIds = activeList.AsReadOnly();
        }
    }

    /// <summary>
    /// Deactivates a rule by its ID (excludes it from availability evaluation).
    /// </summary>
    public void DeactivateRule(RuleId ruleId)
    {
        var activeList = ActiveRuleIds.ToList();
        activeList.RemoveAll(id => id == ruleId);
        ActiveRuleIds = activeList.AsReadOnly();
    }

    /// <summary>
    /// Evaluates availability for a given time range.
    /// Algorithm: among all active rules, the one with the highest effective priority wins.
    /// If no rules match, availability defaults to DENY (fail-closed).
    /// </summary>
    public bool IsAvailable(DateRange timeRange)
    {
        var activeRules = _rules
            .Where(r => ActiveRuleIds.Any(id => id == r.Id))
            .OrderByDescending(r => r.CalculateEffectivePriority(BasePriority))
            .ToList();

        if (activeRules.Count == 0)
            return false;

        // Highest-priority rule determines the outcome
        var winningRule = activeRules.First();
        return winningRule.Effect == Effect.Allow;
    }

    /// <summary>
    /// Returns all active rules, ordered by effective priority (highest first).
    /// Used by CatalogSearchService for rule aggregation across hierarchy.
    /// </summary>
    public IReadOnlyList<RuleDefinition> GetActiveRulesOrdered()
    {
        return _rules
            .Where(r => ActiveRuleIds.Any(id => id == r.Id))
            .OrderByDescending(r => r.CalculateEffectivePriority(BasePriority))
            .ToList()
            .AsReadOnly();
    }
}
