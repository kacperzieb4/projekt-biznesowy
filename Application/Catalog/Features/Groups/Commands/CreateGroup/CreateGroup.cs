using AttractionCatalog.Application.Common.Models;
using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using FluentValidation;
using MediatR;

namespace AttractionCatalog.Application.Catalog.Features.Groups.Commands.CreateGroup;

// --- Command ---

public record CreateGroupCommand(
    string Name,
    SequenceMode SequenceMode,
    List<Guid> ComponentIds) : IRequest<Result<Guid>>;

// --- Validator ---

public class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ComponentIds).NotEmpty();
    }
}

// --- Handler ---

public sealed class CreateGroupHandler : IRequestHandler<CreateGroupCommand, Result<Guid>>
{
    private readonly IAttractionRepository _repository;

    public CreateGroupHandler(IAttractionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var components = new List<IAttractionComponent>();

        foreach (var id in request.ComponentIds)
        {
            var component = await _repository.FindByIdAsync(new AttractionId(id), cancellationToken);
            if (component == null)
            {
                return Result<Guid>.Failure($"Component with ID {id} not found.");
            }
            components.Add(component);
        }

        var groupId = Guid.NewGuid();
        var schedule = new AvailabilitySchedule(0, []);

        var group = new AttractionGroup(
            new AttractionId(groupId),
            request.Name,
            request.SequenceMode,
            [],
            schedule,
            components
        );

        await _repository.SaveAsync(group, cancellationToken);
        
        return Result<Guid>.Success(groupId);
    }
}
