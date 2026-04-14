using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

namespace AttractionCatalog.Domain.Modules.CatalogSearch.Services;

/// <summary>
/// Specification that evaluates whether a given time satisfies a rule's conditions.
/// </summary>
public interface IRuleSpecification
{
    bool IsSatisfiedBy(DateTime time);
}

/// <summary>
/// Compiles a <see cref="RuleDefinition"/> into an executable <see cref="IRuleSpecification"/>.
/// This is the bridge between data-driven rules and actual evaluation logic.
/// </summary>
public class RuleSpecificationCompiler
{
    public IRuleSpecification CompileRule(RuleDefinition rule)
    {
        return new TimeBasedRuleSpecification(rule.Params);
    }

    private sealed class TimeBasedRuleSpecification : IRuleSpecification
    {
        private readonly Dictionary<string, object> _params;

        public TimeBasedRuleSpecification(Dictionary<string, object> @params) => _params = @params;

        public bool IsSatisfiedBy(DateTime time)
        {
            // TODO: Implement actual rule evaluation based on Params.
            // For now, always matches so that priority-based Effect (Allow/Deny) is the deciding factor.
            return true;
        }
    }
}

/// <summary>
/// Domain service that checks availability of attractions and groups recursively.
/// Uses the Composite pattern — groups delegate to children, single attractions delegate to scenarios.
/// </summary>
public class CatalogSearchService
{
    private readonly RuleSpecificationCompiler _compiler;
    private readonly List<RuleDefinition> _globalRules;

    public CatalogSearchService(RuleSpecificationCompiler compiler, List<RuleDefinition> globalRules)
    {
        _compiler = compiler;
        _globalRules = globalRules;
    }

    /// <summary>
    /// Recursively checks whether a component (single attraction or group) is available at a given time.
    /// </summary>
    public bool CheckAvailability(IAttractionComponent component, DateTime time)
    {
        // First check the component's own schedule
        if (!component.Schedule.IsAvailable(time, _globalRules, _compiler))
            return false;

        // Groups: ALL children must be available (logical AND)
        if (component is AttractionGroup group)
            return group.Components.All(child => CheckAvailability(child, time));

        // Single attractions: at least one scenario must be available, or no scenarios = available
        if (component is SingleAttraction single)
            return single.Scenarios.Count == 0 || single.Scenarios.Any(s => s.Schedule.IsAvailable(time, _globalRules, _compiler));

        return true;
    }
}
