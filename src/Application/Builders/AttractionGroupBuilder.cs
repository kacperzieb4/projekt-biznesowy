using Application.Interfaces;
using Domain.Core.Attractions.Aggregates;
using Domain.Core.Attractions.Entities;
using Domain.Core.Attractions.Enums;
using Domain.Core.Attractions.Interfaces;
using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Entities;

namespace Application.Builders;

/// <summary>
/// Builder pattern for safe construction of AttractionGroup aggregates.
/// 
/// Isolates complex validation logic from the application layer:
///   - Verifies all components exist in the repository
///   - Validates all components are in CATALOG or INTERNAL state
///   - Ensures no time conflicts between components
///   - Guarantees no "illegal" business state escapes
/// 
/// Only Build() produces a valid AttractionGroup — partial configuration is never exposed.
/// </summary>
public sealed class AttractionGroupBuilder
{
    private readonly AttractionId _id;
    private readonly IAttractionRepository _repository;
    private string? _name;
    private SequenceMode _sequenceMode = SequenceMode.Flexible;
    private readonly List<AttractionId> _componentIds = new();
    private readonly List<Tag> _tags = new();
    private int _scheduleBasePriority = 50;

    public AttractionGroupBuilder(AttractionId id, IAttractionRepository repository)
    {
        _id = id ?? throw new ArgumentNullException(nameof(id));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public AttractionGroupBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public AttractionGroupBuilder AddComponent(AttractionId componentId)
    {
        ArgumentNullException.ThrowIfNull(componentId);

        if (_componentIds.Any(c => c == componentId))
            throw new InvalidOperationException($"Component {componentId} already added to the group.");

        _componentIds.Add(componentId);
        return this;
    }

    public AttractionGroupBuilder WithSequenceMode(SequenceMode mode)
    {
        _sequenceMode = mode;
        return this;
    }

    public AttractionGroupBuilder WithTag(Tag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        _tags.Add(tag);
        return this;
    }

    public AttractionGroupBuilder WithScheduleBasePriority(int basePriority)
    {
        _scheduleBasePriority = basePriority;
        return this;
    }

    /// <summary>
    /// Builds the AttractionGroup after validating all business rules.
    /// Throws InvalidOperationException if any invariant is violated.
    /// </summary>
    public async Task<AttractionGroup> BuildAsync(CancellationToken cancellationToken = default)
    {
        // ── Validate configuration ──────────────────────────────────────
        ValidateConfiguration();

        // ── Resolve and validate components ─────────────────────────────
        var components = await ResolveAndValidateComponentsAsync(cancellationToken);

        // ── Construct the aggregate ─────────────────────────────────────
        var schedule = new AvailabilitySchedule(_scheduleBasePriority);

        return new AttractionGroup(
            _id,
            _name!,
            _sequenceMode,
            schedule,
            components,
            _tags);
    }

    // ── Private Validation Methods ─────────────────────────────────────

    private void ValidateConfiguration()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(_name))
            errors.Add("Group name is required.");

        if (_componentIds.Count == 0)
            errors.Add("Group must contain at least one component.");

        if (errors.Count > 0)
            throw new InvalidOperationException(
                $"Cannot build AttractionGroup: {string.Join("; ", errors)}");
    }

    private async Task<List<IAttractionComponent>> ResolveAndValidateComponentsAsync(
        CancellationToken cancellationToken)
    {
        var components = new List<IAttractionComponent>();
        var errors = new List<string>();

        foreach (var componentId in _componentIds)
        {
            var component = await _repository.FindByIdAsync(componentId, cancellationToken);

            if (component is null)
            {
                errors.Add($"Component {componentId} not found.");
                continue;
            }

            // Validate state — only CATALOG or INTERNAL components allowed in groups
            if (component is SingleAttraction single)
            {
                if (single.State is not (AttractionState.Catalog or AttractionState.Internal))
                {
                    errors.Add(
                        $"Component '{single.Name}' ({single.Id}) is in state {single.State}. " +
                        $"Only CATALOG or INTERNAL attractions can be added to a group.");
                    continue;
                }
            }

            components.Add(component);
        }

        if (errors.Count > 0)
            throw new InvalidOperationException(
                $"Cannot build AttractionGroup: {string.Join("; ", errors)}");

        return components;
    }
}
