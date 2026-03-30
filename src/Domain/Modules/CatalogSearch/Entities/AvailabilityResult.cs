using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Domain.Modules.CatalogSearch.Entities;

/// <summary>
/// Result of an availability evaluation for a specific attraction component.
/// Returned by CatalogSearchService.FindAvailableAttractions().
/// </summary>
public sealed class AvailabilityResult
{
    /// <summary>ID of the evaluated attraction component.</summary>
    public AttractionId ComponentId { get; }

    /// <summary>Whether the component is available based on all evaluated rules.</summary>
    public bool IsAvailable { get; }

    /// <summary>
    /// IDs of specific scenarios that are available within this component.
    /// Populated only for SingleAttraction types (groups don't have scenarios directly).
    /// </summary>
    public IReadOnlyList<ScenarioId> AvailableScenarios { get; }

    /// <summary>IDs of rules that were applied during evaluation (for audit/debugging).</summary>
    public IReadOnlyList<RuleId> AppliedRules { get; }

    public AvailabilityResult(
        AttractionId componentId,
        bool isAvailable,
        IEnumerable<ScenarioId>? availableScenarios = null,
        IEnumerable<RuleId>? appliedRules = null)
    {
        ComponentId = componentId ?? throw new ArgumentNullException(nameof(componentId));
        IsAvailable = isAvailable;
        AvailableScenarios = availableScenarios?.ToList().AsReadOnly()
            ?? new List<ScenarioId>().AsReadOnly();
        AppliedRules = appliedRules?.ToList().AsReadOnly()
            ?? new List<RuleId>().AsReadOnly();
    }
}
