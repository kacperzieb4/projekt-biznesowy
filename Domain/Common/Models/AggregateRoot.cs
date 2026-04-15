namespace AttractionCatalog.Domain.Common.Models;

/// <summary>
/// Base record for all domain events. Carries a timestamp of when it occurred.
/// </summary>
public abstract record DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Base class for aggregate roots. Collects domain events that are dispatched
/// after a successful persistence operation.
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<DomainEvent> _events = [];

    public IReadOnlyCollection<DomainEvent> Events => _events;

    protected void AddEvent(DomainEvent @event) => _events.Add(@event);
    public void ClearEvents() => _events.Clear();
}
