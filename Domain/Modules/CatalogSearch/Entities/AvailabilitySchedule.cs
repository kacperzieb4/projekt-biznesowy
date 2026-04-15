using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;
using AttractionCatalog.Domain.Modules.CatalogSearch.Services;

namespace AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

/// <summary>
/// Defines when an attraction/scenario/group is available, using a set of priority-based rules.
/// The rule with the highest combined priority (BasePriority + Rule.Priority) wins.
/// </summary>
public class AvailabilitySchedule
{
    public int BasePriority { get; }
    private readonly List<RuleId> _activeRuleIds;

    public IReadOnlyCollection<RuleId> ActiveRuleIds => _activeRuleIds;

    public AvailabilitySchedule(int basePriority, IEnumerable<RuleId> rules)
    {
        BasePriority = basePriority;
        _activeRuleIds = rules.ToList();
    }

    /// <summary>
    /// Evaluates whether this schedule allows availability at the given time.
    /// Rules are sorted by combined priority (descending); the first matching rule wins.
    /// If no rule matches, defaults to available.
    /// </summary>
    public bool IsAvailable(DateTime time, IEnumerable<RuleDefinition> allRules, RuleSpecificationCompiler compiler)
    {
        var applicableRules = allRules
            .Where(r => _activeRuleIds.Contains(r.Id))
            .Select(r => new { Rule = r, Priority = BasePriority + r.Priority })
            .OrderByDescending(r => r.Priority)
            .ToList();

        foreach (var r in applicableRules)
        {
            var spec = compiler.CompileRule(r.Rule);
            if (spec.IsSatisfiedBy(time))
            {
                return r.Rule.Effect == Effect.Allow;
            }
        }

        // No rule matched — default to available
        return true;
    }
}
