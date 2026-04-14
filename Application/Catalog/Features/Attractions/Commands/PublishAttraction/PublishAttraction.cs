using AttractionCatalog.Application.Common.Models;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using MediatR;

namespace AttractionCatalog.Application.Catalog.Features.Attractions.Commands.PublishAttraction;

// --- Command ---

public record PublishAttractionCommand(Guid Id) : IRequest<Result>;

// --- Handler ---

public sealed class PublishAttractionHandler : IRequestHandler<PublishAttractionCommand, Result>
{
    private readonly IAttractionRepository _repository;

    public PublishAttractionHandler(IAttractionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(PublishAttractionCommand request, CancellationToken cancellationToken)
    {
        var component = await _repository.FindByIdAsync(new AttractionId(request.Id), cancellationToken);

        if (component == null)
        {
            return Result.Failure("Attraction not found.");
        }

        if (component is not SingleAttraction singleAttraction)
        {
            return Result.Failure("Only single attractions can be published directly.");
        }

        singleAttraction.Publish();

        await _repository.SaveAsync(singleAttraction, cancellationToken);
        
        return Result.Success();
    }
}
