using Domain.Modules.CatalogSearch.Enums;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Domain.Modules.CatalogSearch.Entities;

/// <summary>
/// Data-driven rule definition for availability evaluation.
/// Rules are stored as structured data (not hardcoded logic), enabling
/// dynamic modification of service parameters without system downtime.
/// </summary>
public sealed class RuleDefinition
{
    public RuleId Id { get; }

    /// <summary>Classification: Weekly, Seasonal, or Exception.</summary>
    public RuleType Type { get; }

    /// <summary>
    /// Local priority within the owning AvailabilitySchedule.
    /// Higher values override lower values. Effective priority = BasePriority + Priority.
    /// </summary>
    public int Priority { get; }

    /// <summary>Whether this rule allows or denies availability.</summary>
    public Effect Effect { get; }

    /// <summary>
    /// Flexible parameter bag for rule-specific configuration (e.g., days of week, time ranges, date ranges).
    /// </summary>
    public IReadOnlyDictionary<string, object> Params { get; }

    public RuleDefinition(RuleId id, RuleType type, int priority, Effect effect, Dictionary<string, object>? parameters = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Type = type;
        Priority = priority;
        Effect = effect;
        Params = parameters is not null
            ? new Dictionary<string, object>(parameters)
            : new Dictionary<string, object>();
    }

    /// <summary>
    /// Factory method for creating a new rule with a generated ID.
    /// </summary>
    public static RuleDefinition Create(
        RuleType type,
        int priority,
        Effect effect,
        Dictionary<string, object>? parameters = null)
        => new(RuleId.CreateNew(), type, priority, effect, parameters);

    /// <summary>
    /// Calculates the effective priority of this rule within its schedule context.
    /// Used during rule competition resolution across hierarchy levels.
    /// </summary>
    public int CalculateEffectivePriority(int basePriority) => basePriority + Priority;
}
