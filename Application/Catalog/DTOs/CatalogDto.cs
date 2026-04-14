namespace AttractionCatalog.Application.Catalog.DTOs;

public record CatalogDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsAvailable { get; init; }
}
