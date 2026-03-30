using Domain.Core.Attractions.Enums;
using Domain.Core.Attractions.Interfaces;
using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Entities;

namespace Domain.Core.Attractions.Aggregates;

/// <summary>
/// Composite aggregate — groups multiple IAttractionComponent instances into a package.
/// 
/// Implements Composite pattern: treated uniformly with SingleAttraction via IAttractionComponent.
/// 
/// Business rules:
///   - All components must be in CATALOG or INTERNAL state.
///   - Group availability = own schedule ALLOW AND all children available.
///   - SequenceMode defines ordering constraints (STRICT/FLEXIBLE/NONE).
/// 
/// Created exclusively through AttractionGroupBuilder to guarantee invariant enforcement.
/// </summary>
public sealed class AttractionGroup : IAttractionComponent
{
    private readonly List<Tag> _tags = new();
    private readonly List<IAttractionComponent> _components = new();

    public AttractionId Id { get; }
    public string Name { get; private set; }

    /// <summary>Ordering constraint for visiting components.</summary>
    public SequenceMode SequenceMode { get; private set; }

    /// <summary>Group-level tags.</summary>
    public IReadOnlyList<Tag> Tags => _tags.AsReadOnly();

    /// <summary>
    /// Group-level availability schedule.
    /// For groups: IsAvailable = OwnRules.Allow AND All(Children.IsAvailable).
    /// </summary>
    public AvailabilitySchedule Schedule { get; }

    /// <summary>
    /// Aggregated child components (SingleAttractions, or nested AttractionGroups).
    /// </summary>
    public IReadOnlyList<IAttractionComponent> Components => _components.AsReadOnly();

    /// <summary>
    /// Internal constructor — only AttractionGroupBuilder should create instances
    /// to ensure all business invariants are validated before instantiation.
    /// </summary>
    internal AttractionGroup(
        AttractionId id,
        string name,
        SequenceMode sequenceMode,
        AvailabilitySchedule schedule,
        IEnumerable<IAttractionComponent> components,
        IEnumerable<Tag>? tags = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new ArgumentException("Group name cannot be empty.", nameof(name));
        SequenceMode = sequenceMode;
        Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
        _components = components?.ToList()
            ?? throw new ArgumentNullException(nameof(components));

        if (tags is not null)
            _tags = tags.ToList();
    }

    // ── Tag Management ─────────────────────────────────────────────────

    public void AddTag(Tag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);

        if (_tags.Any(t => t.Id == tag.Id))
            throw new InvalidOperationException($"Tag {tag.Code} already assigned to group {Name}.");

        _tags.Add(tag);
    }

    public void RemoveTag(TagId tagId)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is not null)
            _tags.Remove(tag);
    }

    // ── Queries ────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the total number of components (recursive, including nested groups' children).
    /// </summary>
    public int GetTotalComponentCount()
    {
        return _components.Sum(c => c is AttractionGroup group
            ? 1 + group.GetTotalComponentCount()
            : 1);
    }
}
