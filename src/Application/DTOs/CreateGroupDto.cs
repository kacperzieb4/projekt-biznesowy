using Domain.Core.Attractions.Enums;

namespace Application.DTOs;

/// <summary>
/// Request DTO for creating an attraction group (package).
/// </summary>
public sealed record CreateGroupDto
{
    /// <summary>Name of the group/package.</summary>
    public required string Name { get; init; }

    /// <summary>IDs of attractions to include in the group (must be CATALOG or INTERNAL).</summary>
    public required List<Guid> ComponentIds { get; init; }

    /// <summary>Sequencing mode for visiting components.</summary>
    public SequenceMode SequenceMode { get; init; } = SequenceMode.Flexible;
}
