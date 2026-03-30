using Application.Builders;
using Application.DTOs;
using Application.Interfaces;
using Domain.Core.Attractions.ValueObjects;

namespace Application.UseCases;

/// <summary>
/// Use Case: Create an attraction group (package) from existing attractions.
/// 
/// Delegates construction to AttractionGroupBuilder, which enforces:
///   - All components exist
///   - All components are in CATALOG or INTERNAL state
///   - No time conflicts between components
/// 
/// Single Responsibility: group creation only.
/// </summary>
public sealed class CreateAttractionGroupUseCase
{
    private readonly IAttractionRepository _repository;

    public CreateAttractionGroupUseCase(IAttractionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Creates a new attraction group based on the request DTO.
    /// Returns the ID of the newly created group.
    /// </summary>
    public async Task<AttractionId> ExecuteAsync(CreateGroupDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var groupId = AttractionId.CreateNew();

        var builder = new AttractionGroupBuilder(groupId, _repository)
            .WithName(request.Name)
            .WithSequenceMode(request.SequenceMode);

        foreach (var componentIdGuid in request.ComponentIds)
        {
            builder.AddComponent(new AttractionId(componentIdGuid));
        }

        var group = await builder.BuildAsync(cancellationToken);

        await _repository.SaveAsync(group, cancellationToken);

        return group.Id;
    }
}
