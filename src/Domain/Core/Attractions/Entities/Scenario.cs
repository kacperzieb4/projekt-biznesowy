using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Entities;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Domain.Core.Attractions.Entities;

/// <summary>
/// Entity representing a specific variant/way of experiencing a SingleAttraction.
/// E.g., "Tourist Route" vs "Mining Route" for the Wieliczka Salt Mine.
/// 
/// Each Scenario has its own duration, tags, and availability schedule.
/// Availability is intersection-based: both the parent SingleAttraction
/// AND the Scenario must be available for the variant to show in search results.
/// </summary>
public sealed class Scenario
{
    private readonly List<Tag> _tags = new();

    public ScenarioId Id { get; }
    public string Name { get; private set; }

    /// <summary>Estimated duration for this variant experience.</summary>
    public TimeSpan Duration { get; private set; }

    /// <summary>Scenario-specific tags (merged with parent tags in read projections).</summary>
    public IReadOnlyList<Tag> Tags => _tags.AsReadOnly();

    /// <summary>
    /// Scenario-level availability schedule (specific rules).
    /// Intersected with parent attraction's global rules during availability evaluation.
    /// </summary>
    public AvailabilitySchedule Schedule { get; }

    public Scenario(ScenarioId id, string name, TimeSpan duration, AvailabilitySchedule schedule)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new ArgumentException("Scenario name cannot be empty.", nameof(name));
        Duration = duration > TimeSpan.Zero
            ? duration
            : throw new ArgumentException("Scenario duration must be positive.", nameof(duration));
        Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
    }

    /// <summary>
    /// Factory method creating a Scenario with default schedule and generated ID.
    /// </summary>
    public static Scenario Create(string name, TimeSpan duration, int scheduleBasePriority = 200)
        => new(ScenarioId.CreateNew(), name, duration, new AvailabilitySchedule(scheduleBasePriority));

    public void AddTag(Tag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);

        if (_tags.Any(t => t.Id == tag.Id))
            throw new InvalidOperationException($"Tag {tag.Code} already assigned to scenario {Name}.");

        _tags.Add(tag);
    }

    public void RemoveTag(TagId tagId)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is not null)
            _tags.Remove(tag);
    }

    public void UpdateName(string name)
    {
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new ArgumentException("Scenario name cannot be empty.", nameof(name));
    }

    public void UpdateDuration(TimeSpan duration)
    {
        Duration = duration > TimeSpan.Zero
            ? duration
            : throw new ArgumentException("Scenario duration must be positive.", nameof(duration));
    }
}
