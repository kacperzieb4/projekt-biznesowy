using System;
using System.Collections.Generic;

namespace AttractionCatalog.Domain.Common.Models
{
    public abstract record DomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public abstract class AggregateRoot
    {
        private readonly List<DomainEvent> _events = new();
        public IReadOnlyCollection<DomainEvent> Events => _events;

        protected void AddEvent(DomainEvent @event) => _events.Add(@event);
        public void ClearEvents() => _events.Clear();
    }
}
