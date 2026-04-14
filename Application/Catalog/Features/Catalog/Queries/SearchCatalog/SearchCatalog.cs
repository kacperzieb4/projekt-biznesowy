using AttractionCatalog.Application.Catalog.DTOs;
using AttractionCatalog.Application.Catalog.Specifications;
using AttractionCatalog.Application.Common.Models;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Services;
using AttractionCatalog.Domain.Modules.CatalogSearch.ValueObjects;
using MediatR;

namespace AttractionCatalog.Application.Catalog.Features.Catalog.Queries.SearchCatalog;

// --- Query ---

public record SearchCatalogQuery(
    DateTime Start,
    DateTime End,
    double? Lat,
    double? Lon,
    double? Radius,
    List<Guid>? RequiredTags,
    List<Guid>? ExcludedTags) : IRequest<Result<List<CatalogDto>>>;

// --- Handler ---

public sealed class SearchCatalogHandler : IRequestHandler<SearchCatalogQuery, Result<List<CatalogDto>>>
{
    private readonly IAttractionRepository _repository;
    private readonly CatalogSearchService _searchService;

    public SearchCatalogHandler(IAttractionRepository repository, CatalogSearchService searchService)
    {
        _repository = repository;
        _searchService = searchService;
    }

    public async Task<Result<List<CatalogDto>>> Handle(SearchCatalogQuery request, CancellationToken cancellationToken)
    {
        // Build the right specification based on query parameters
        IQuerySpecification<IAttractionComponent> spec = request.Lat.HasValue
            ? new LocationQuerySpecification(new GeoArea(request.Lat.Value, request.Lon!.Value, request.Radius!.Value))
            : new AllAttractionsSpecification();

        var candidates = await _repository.FindByCriteriaAsync(spec, cancellationToken);

        // Apply availability filtering via domain service
        var available = candidates
            .Where(c => _searchService.CheckAvailability(c, request.Start))
            .ToList();

        return Result<List<CatalogDto>>.Success(available.MapToDto());
    }
}

// --- Mapper ---

public static class SearchCatalogMapper
{
    public static List<CatalogDto> MapToDto(this List<IAttractionComponent> components)
    {
        return components.Select(c => new CatalogDto
        {
            Id = c.Id.Value,
            Name = c.Name,
            IsAvailable = true
        }).ToList();
    }
}
