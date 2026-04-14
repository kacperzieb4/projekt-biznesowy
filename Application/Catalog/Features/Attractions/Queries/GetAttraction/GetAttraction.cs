using AttractionCatalog.Application.Catalog.DTOs;
using AttractionCatalog.Application.Common.Models;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using MediatR;

namespace AttractionCatalog.Application.Catalog.Features.Attractions.Queries.GetAttraction;

// --- Query ---

public record GetAttractionQuery(Guid Id) : IRequest<Result<CatalogDto>>;

// --- Handler ---

public sealed class GetAttractionHandler : IRequestHandler<GetAttractionQuery, Result<CatalogDto>>
{
    private readonly IAttractionRepository _repository;

    public GetAttractionHandler(IAttractionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CatalogDto>> Handle(GetAttractionQuery request, CancellationToken cancellationToken)
    {
        var attraction = await _repository.FindByIdAsync(new AttractionId(request.Id), cancellationToken);

        if (attraction == null)
        {
            return Result<CatalogDto>.Failure("Attraction not found.");
        }

        return Result<CatalogDto>.Success(new CatalogDto
        {
            Id = attraction.Id.Value,
            Name = attraction.Name,
            IsAvailable = true
        });
    }
}
