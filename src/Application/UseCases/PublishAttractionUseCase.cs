using Application.Interfaces;
using Domain.Core.Attractions.Entities;
using Domain.Core.Attractions.ValueObjects;

namespace Application.UseCases;

/// <summary>
/// Use Case: Publish an attraction from DRAFT to CATALOG state.
/// 
/// Orchestration flow:
/// 1. Fetch attraction from repository
/// 2. Validate that attraction can be published (domain method with built-in invariants)
/// 3. Call Publish() on the domain entity
/// 4. Persist updated entity
/// 
/// The actual validation logic lives in the domain entity (SingleAttraction.Publish()),
/// keeping this use case thin — it only orchestrates the flow.
/// </summary>
public sealed class PublishAttractionUseCase
{
    private readonly IAttractionRepository _repository;

    public PublishAttractionUseCase(IAttractionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Publishes the specified attraction.
    /// Throws if the attraction doesn't exist or doesn't meet publication criteria.
    /// </summary>
    public async Task ExecuteAsync(AttractionId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        // 1. Fetch
        var component = await _repository.FindByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Attraction {id} not found.");

        if (component is not SingleAttraction attraction)
            throw new InvalidOperationException(
                $"Entity {id} is not a SingleAttraction. Only individual attractions can be published.");

        // 2. Domain-level state transition with invariant validation
        attraction.Publish();

        // 3. Persist
        await _repository.SaveAsync(attraction, cancellationToken);
    }
}
