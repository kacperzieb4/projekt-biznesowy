namespace Application.DTOs;

/// <summary>
/// Request DTO for creating a new attraction.
/// Anemic structure — carries no behavior (Clean Architecture principle).
/// </summary>
public sealed record CreateAttractionDto
{
    /// <summary>Name of the attraction.</summary>
    public required string Name { get; init; }

    /// <summary>Physical latitude of the attraction.</summary>
    public required double Latitude { get; init; }

    /// <summary>Physical longitude of the attraction.</summary>
    public required double Longitude { get; init; }

    /// <summary>Optional initial scenarios to create with the attraction.</summary>
    public List<CreateScenarioDto> Scenarios { get; init; } = new();

    /// <summary>Optional tag codes to assign initially.</summary>
    public List<string> TagCodes { get; init; } = new();
}

/// <summary>
/// DTO for creating a scenario within an attraction.
/// </summary>
public sealed record CreateScenarioDto
{
    /// <summary>Name of the scenario variant.</summary>
    public required string Name { get; init; }

    /// <summary>Estimated duration in minutes.</summary>
    public required int DurationMinutes { get; init; }

    /// <summary>Optional tag codes specific to this scenario.</summary>
    public List<string> TagCodes { get; init; } = new();
}
