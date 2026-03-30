using Application.DTOs;
using Application.Interfaces;
using Domain.Core.Attractions.Entities;
using Domain.Core.Attractions.ValueObjects;

namespace Application.UseCases;

/// <summary>
/// Use Case: Create a new tourist attraction in DRAFT state.
/// 
/// Orchestration flow:
/// 1. Validate request DTO
/// 2. Create domain entity (SingleAttraction) in Draft state via factory method
/// 3. Attach initial scenarios and tags
/// 4. Persist through repository
/// 
/// Single Responsibility: only handles attraction creation — publishing is a separate use case.
/// </summary>
public sealed class CreateAttractionUseCase
{
    private readonly IAttractionRepository _repository;

    public CreateAttractionUseCase(IAttractionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Executes the attraction creation use case.
    /// Returns the ID of the newly created attraction.
    /// </summary>
    public async Task<AttractionId> ExecuteAsync(CreateAttractionDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);

        // 1. Create entity in Draft state
        var location = new Location(request.Latitude, request.Longitude);
        var attraction = SingleAttraction.Create(request.Name, location);

        // 2. Attach initial scenarios
        foreach (var scenarioDto in request.Scenarios)
        {
            var scenario = Scenario.Create(
                scenarioDto.Name,
                TimeSpan.FromMinutes(scenarioDto.DurationMinutes));

            // Add scenario-level tags
            foreach (var tagCode in scenarioDto.TagCodes)
            {
                var tag = Tag.Create(tagCode, tagCode);
                scenario.AddTag(tag);
            }

            attraction.AddScenario(scenario);
        }

        // 3. Attach initial tags
        foreach (var tagCode in request.TagCodes)
        {
            var tag = Tag.Create(tagCode, tagCode);
            attraction.AddTag(tag);
        }

        // 4. Persist
        await _repository.SaveAsync(attraction, cancellationToken);

        return attraction.Id;
    }

    private static void ValidateRequest(CreateAttractionDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Name is required.");

        foreach (var scenario in request.Scenarios)
        {
            if (string.IsNullOrWhiteSpace(scenario.Name))
                errors.Add("Scenario name is required.");
            if (scenario.DurationMinutes <= 0)
                errors.Add($"Scenario '{scenario.Name}' duration must be positive.");
        }

        if (errors.Count > 0)
            throw new ArgumentException($"Invalid request: {string.Join("; ", errors)}");
    }
}
