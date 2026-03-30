using Domain.Core.Attractions.Enums;
using Domain.Core.Attractions.Interfaces;
using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Entities;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Domain.Core.Attractions.Entities;

/// <summary>
/// Root aggregate for individual tourist attractions (museums, events, cruises, trails, tours).
/// 
/// Implements IAttractionComponent (Composite pattern) to be uniformly treatable
/// alongside AttractionGroup.
/// 
/// Lifecycle: DRAFT → CATALOG/INTERNAL → ARCHIVED
/// Encapsulates invariants:
///   - Cannot publish without a name, location, and at least one scenario
///   - State transitions follow the defined lifecycle
///   - Tags and scenarios are managed through domain methods
/// </summary>
public sealed class SingleAttraction : IAttractionComponent
{
    private readonly List<Tag> _tags = new();
    private readonly List<Scenario> _scenarios = new();

    public AttractionId Id { get; }
    public string Name { get; private set; }

    /// <summary>Current lifecycle state of the attraction.</summary>
    public AttractionState State { get; private set; }

    /// <summary>Physical location on the map.</summary>
    public Location Location { get; private set; }

    /// <summary>Global tags for categorization.</summary>
    public IReadOnlyList<Tag> Tags => _tags.AsReadOnly();

    /// <summary>
    /// Global availability schedule (applies to ALL scenarios).
    /// E.g., "Closed on May 1st" — blocks all scenarios.
    /// </summary>
    public AvailabilitySchedule Schedule { get; }

    /// <summary>
    /// Variants/ways to experience this attraction.
    /// Each scenario has its own schedule intersected with the global one.
    /// </summary>
    public IReadOnlyList<Scenario> Scenarios => _scenarios.AsReadOnly();

    private SingleAttraction(
        AttractionId id,
        string name,
        Location location,
        AvailabilitySchedule schedule)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new ArgumentException("Attraction name cannot be empty.", nameof(name));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
        State = AttractionState.Draft;
    }

    /// <summary>
    /// Factory method: creates a new attraction in DRAFT state with auto-generated ID.
    /// </summary>
    public static SingleAttraction Create(string name, Location location, int scheduleBasePriority = 100)
    {
        return new SingleAttraction(
            AttractionId.CreateNew(),
            name,
            location,
            new AvailabilitySchedule(scheduleBasePriority));
    }

    // ── State Transitions ──────────────────────────────────────────────

    /// <summary>
    /// Publishes the attraction to the catalog (DRAFT → CATALOG).
    /// Validates business rules: must have name, location, and at least one scenario.
    /// </summary>
    public void Publish()
    {
        EnsureCanTransitionFrom(AttractionState.Draft);
        ValidateReadyForPublication();
        State = AttractionState.Catalog;
    }

    /// <summary>
    /// Marks the attraction as internal-only (DRAFT → INTERNAL).
    /// Available for usage in groups, but not for direct purchase.
    /// </summary>
    public void MarkAsInternal()
    {
        EnsureCanTransitionFrom(AttractionState.Draft);
        ValidateReadyForPublication();
        State = AttractionState.Internal;
    }

    /// <summary>
    /// Archives the attraction (CATALOG/INTERNAL → ARCHIVED).
    /// </summary>
    public void Archive()
    {
        if (State is not (AttractionState.Catalog or AttractionState.Internal))
            throw new InvalidOperationException(
                $"Cannot archive attraction in state {State}. Must be Catalog or Internal.");

        State = AttractionState.Archived;
    }

    // ── Scenario Management ────────────────────────────────────────────

    public void AddScenario(Scenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        if (_scenarios.Any(s => s.Id == scenario.Id))
            throw new InvalidOperationException($"Scenario {scenario.Id} already exists.");

        _scenarios.Add(scenario);
    }

    public void RemoveScenario(ScenarioId scenarioId)
    {
        var scenario = _scenarios.FirstOrDefault(s => s.Id == scenarioId)
            ?? throw new InvalidOperationException($"Scenario {scenarioId} not found.");

        _scenarios.Remove(scenario);
    }

    // ── Tag Management ─────────────────────────────────────────────────

    public void AddTag(Tag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);

        if (_tags.Any(t => t.Id == tag.Id))
            throw new InvalidOperationException($"Tag {tag.Code} already assigned.");

        _tags.Add(tag);
    }

    public void RemoveTag(TagId tagId)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is not null)
            _tags.Remove(tag);
    }

    // ── Update Methods ─────────────────────────────────────────────────

    public void UpdateName(string name)
    {
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new ArgumentException("Attraction name cannot be empty.", nameof(name));
    }

    public void UpdateLocation(Location location)
    {
        Location = location ?? throw new ArgumentNullException(nameof(location));
    }

    // ── Private Helpers ────────────────────────────────────────────────

    private void EnsureCanTransitionFrom(AttractionState requiredState)
    {
        if (State != requiredState)
            throw new InvalidOperationException(
                $"Cannot transition from {State}. Required state: {requiredState}.");
    }

    private void ValidateReadyForPublication()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required.");

        if (Location is null)
            errors.Add("Location is required.");

        if (_scenarios.Count == 0)
            errors.Add("At least one scenario is required for publication.");

        if (errors.Count > 0)
            throw new InvalidOperationException(
                $"Attraction cannot be published: {string.Join("; ", errors)}");
    }
}
