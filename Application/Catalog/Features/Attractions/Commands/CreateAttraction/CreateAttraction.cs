using AttractionCatalog.Application.Common.Models;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using FluentValidation;
using MediatR;

namespace AttractionCatalog.Application.Catalog.Features.Attractions.Commands.CreateAttraction;

// --- Command ---

public record CreateAttractionCommand(
    string Name,
    Location Location,
    List<Guid> TagIds) : IRequest<Result<Guid>>;

// --- Validator ---

public class CreateAttractionValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Location).NotNull();
    }
}

// --- Handler ---

public sealed class CreateAttractionHandler : IRequestHandler<CreateAttractionCommand, Result<Guid>>
{
    private readonly IAttractionRepository _repository;

    public CreateAttractionHandler(IAttractionRepository repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateAttractionCommand request, CancellationToken cancellationToken)
    {
        var attractionId = Guid.NewGuid();
        var attraction = new SingleAttraction(
            new AttractionId(attractionId),
            request.Name,
            AttractionState.Draft,
            [],
            request.Location,
            new AvailabilitySchedule(0, []),
            []);

        attraction.Publish();

        await _repository.SaveAsync(attraction, cancellationToken);
        return Result<Guid>.Success(attractionId);
    }
}
