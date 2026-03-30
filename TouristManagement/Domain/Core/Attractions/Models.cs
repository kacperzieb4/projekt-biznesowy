using TouristManagement.Domain.Modules.CatalogSearch;

namespace TouristManagement.Domain.Core.Attractions;

public record AttractionId(Guid Value) { public static AttractionId New() => new(Guid.NewGuid()); }
public record TagId(Guid Value) { public static TagId New() => new(Guid.NewGuid()); }
public record ScenarioId(Guid Value) { public static ScenarioId New() => new(Guid.NewGuid()); }
public record Location(double Latitude, double Longitude);

public enum AttractionState { Draft, Catalog, Internal, Archived }
public enum RelationType { Requires, Excludes, RecommendedAfter }
public enum SequenceMode { Strict, Flexible, None }

public interface IAttractionComponent
{
    AttractionId Id { get; }
    string Name { get; }
    List<TagId> Tags { get; }
    AvailabilitySchedule Schedule { get; }
    AttractionState State { get; }
}

public class Tag
{
    public TagId Id { get; init; }
    public string Code { get; init; }
    public string DisplayName { get; init; }
    public string Description { get; init; }
}

public class SingleAttraction : IAttractionComponent
{
    public AttractionId Id { get; }
    public string Name { get; private set; }
    public AttractionState State { get; private set; }
    public List<TagId> Tags { get; } = new();
    public Location Location { get; private set; }
    public AvailabilitySchedule Schedule { get; } = new();
    public List<Scenario> Scenarios { get; } = new();

    public SingleAttraction(AttractionId id, string name, Location location)
    {
        Id = id; Name = name; Location = location; State = AttractionState.Draft;
    }

    public void Publish() => State = AttractionState.Catalog;
    public void AddScenario(Scenario scenario) => Scenarios.Add(scenario);
}

public class Scenario
{
    public ScenarioId Id { get; }
    public string Name { get; }
    public TimeSpan Duration { get; }
    public List<TagId> Tags { get; } = new();
    public AvailabilitySchedule Schedule { get; } = new();

    public Scenario(ScenarioId id, string name, TimeSpan duration)
    {
        Id = id; Name = name; Duration = duration;
    }
}

public class AttractionGroup : IAttractionComponent
{
    public AttractionId Id { get; }
    public string Name { get; }
    public SequenceMode SequenceMode { get; }
    public List<TagId> Tags { get; } = new();
    public AvailabilitySchedule Schedule { get; } = new();
    public List<IAttractionComponent> Children { get; }
    public AttractionState State { get; private set; } = AttractionState.Catalog;

    public AttractionGroup(AttractionId id, string name, SequenceMode mode, List<IAttractionComponent> children)
    {
        Id = id; Name = name; SequenceMode = mode; Children = children;
    }
}

public class AttractionRelation
{
    public AttractionId From { get; init; }
    public AttractionId To { get; init; }
    public RelationType Type { get; init; }
}