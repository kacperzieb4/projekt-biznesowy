using System.Collections.Concurrent;
using AttractionCatalog.Domain.Common.Models;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;

namespace AttractionCatalog.Infrastructure.Core.Attractions.Adapters;

/// <summary>
/// In-memory implementation of <see cref="IAttractionRepository"/> for development and testing.
/// In production, this would be replaced with an EF Core or Dapper-backed implementation.
/// </summary>
public sealed class InMemoryAttractionRepository : IAttractionRepository
{
    private readonly ConcurrentDictionary<AttractionId, IAttractionComponent> _store = new();

    public Task SaveAsync(IAttractionComponent attraction, CancellationToken ct = default)
    {
        _store[attraction.Id] = attraction;

        if (attraction is AggregateRoot aggregate)
        {
            DispatchEvents(aggregate);
        }

        return Task.CompletedTask;
    }

    public Task<IAttractionComponent?> FindByIdAsync(AttractionId id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var attraction);
        return Task.FromResult(attraction);
    }

    public Task<List<IAttractionComponent>> FindByCriteriaAsync(
        IQuerySpecification<IAttractionComponent> spec,
        CancellationToken ct = default)
    {
        var predicate = spec.ToExpression().Compile();
        var results = _store.Values.Where(predicate).ToList();
        return Task.FromResult(results);
    }

    private static void DispatchEvents(AggregateRoot aggregate)
    {
        foreach (var domainEvent in aggregate.Events)
        {
            // In production: publish to MediatR INotificationHandler or message bus
            System.Diagnostics.Debug.WriteLine(
                $"[DomainEvent] {domainEvent.GetType().Name} at {domainEvent.OccurredOn:O}");
        }

        aggregate.ClearEvents();
    }
}
