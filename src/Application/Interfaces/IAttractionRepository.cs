using Domain.Core.Attractions.Interfaces;
using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Specifications;

namespace Application.Interfaces;

/// <summary>
/// Repository abstraction for attraction components (part of the Application layer).
/// Follows Dependency Inversion Principle — the domain/application layers depend on this
/// abstraction, while the Infrastructure layer provides the concrete implementation.
/// </summary>
public interface IAttractionRepository
{
    /// <summary>Persists an attraction component (create or update).</summary>
    Task SaveAsync(IAttractionComponent attraction, CancellationToken cancellationToken = default);

    /// <summary>Retrieves an attraction component by its strongly-typed ID.</summary>
    Task<IAttractionComponent?> FindByIdAsync(AttractionId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves attraction components matching pre-filter criteria.
    /// Uses IQuerySpecification for database-level filtering (performance optimization).
    /// </summary>
    Task<IReadOnlyList<IAttractionComponent>> FindByCriteriaAsync(
        IQuerySpecification<IAttractionComponent>? specification = null,
        CancellationToken cancellationToken = default);
}
