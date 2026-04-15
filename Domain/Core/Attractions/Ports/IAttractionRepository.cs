using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;

namespace AttractionCatalog.Domain.Core.Attractions.Ports;

/// <summary>
/// Repository port for attraction persistence. Infrastructure layer provides the implementation.
/// </summary>
public interface IAttractionRepository
{
    Task SaveAsync(IAttractionComponent attraction, CancellationToken ct = default);
    Task<IAttractionComponent?> FindByIdAsync(AttractionId id, CancellationToken ct = default);
    Task<List<IAttractionComponent>> FindByCriteriaAsync(IQuerySpecification<IAttractionComponent> spec, CancellationToken ct = default);
}
