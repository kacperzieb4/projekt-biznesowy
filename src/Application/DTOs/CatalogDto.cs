namespace Application.DTOs;

/// <summary>
/// Response DTO for catalog search results.
/// Anemic data transfer structure — no business logic.
/// </summary>
public sealed record CatalogDto
{
    /// <summary>Attraction component ID.</summary>
    public required Guid Id { get; init; }

    /// <summary>Display name of the attraction or group.</summary>
    public required string Name { get; init; }

    /// <summary>Whether the attraction is currently available.</summary>
    public required bool IsAvailable { get; init; }

    /// <summary>Available scenarios within this attraction.</summary>
    public List<CatalogScenarioDto> AvailableScenarios { get; init; } = new();

    /// <summary>Tags associated with this attraction.</summary>
    public List<CatalogTagDto> Tags { get; init; } = new();
}

/// <summary>
/// Scenario projection in catalog results.
/// </summary>
public sealed record CatalogScenarioDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int DurationMinutes { get; init; }
    public List<CatalogTagDto> Tags { get; init; } = new();
}

/// <summary>
/// Tag projection in catalog results.
/// </summary>
public sealed record CatalogTagDto
{
    public required string Code { get; init; }
    public required string DisplayName { get; init; }
}
