using System.Collections.Concurrent;
using Application.Interfaces;
using Domain.Core.Attractions.Interfaces;
using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Specifications;

namespace Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of IAttractionRepository.
/// Uses ConcurrentDictionary for thread-safe operations — suitable for PoC/testing.
/// 
/// In production, this would be replaced with a database-backed implementation
/// (e.g., EF Core, Dapper) while the application/domain layers remain unchanged
/// (Dependency Inversion Principle).
/// </summary>
public sealed class InMemoryAttractionRepository : IAttractionRepository
{
    private readonly ConcurrentDictionary<AttractionId, IAttractionComponent> _data = new();

    public Task SaveAsync(IAttractionComponent attraction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attraction);
        _data.AddOrUpdate(attraction.Id, attraction, (_, _) => attraction);
        return Task.CompletedTask;
    }

    public Task<IAttractionComponent?> FindByIdAsync(AttractionId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        _data.TryGetValue(id, out var result);
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<IAttractionComponent>> FindByCriteriaAsync(
        IQuerySpecification<IAttractionComponent>? specification = null,
        CancellationToken cancellationToken = default)
    {
        var query = _data.Values.AsEnumerable();

        if (specification is not null)
        {
            var predicate = specification.ToExpression();
            query = query.Where(predicate);
        }

        IReadOnlyList<IAttractionComponent> result = query.ToList().AsReadOnly();
        return Task.FromResult(result);
    }

    // ── Utility methods for testing ────────────────────────────────────

    /// <summary>Returns the total number of stored entities.</summary>
    public int Count => _data.Count;

    /// <summary>Clears all stored entities (for testing purposes).</summary>
    public void Clear() => _data.Clear();
}
