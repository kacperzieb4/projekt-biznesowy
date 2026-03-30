using Domain.Core.Attractions.ValueObjects;

namespace Domain.Core.Attractions;

/// <summary>
/// Entity representing a categorization tag.
/// Tags follow an Entity-Attribute-Value pattern for flexible categorization
/// of attractions and scenarios (e.g., "Audio-guide", "Edukacyjne", "Outdoor").
/// </summary>
public sealed class Tag
{
    public TagId Id { get; }
    public string Code { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }

    public Tag(TagId id, string code, string displayName, string description)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Code = !string.IsNullOrWhiteSpace(code)
            ? code
            : throw new ArgumentException("Tag code cannot be empty.", nameof(code));
        DisplayName = !string.IsNullOrWhiteSpace(displayName)
            ? displayName
            : throw new ArgumentException("Tag display name cannot be empty.", nameof(displayName));
        Description = description ?? string.Empty;
    }

    /// <summary>
    /// Factory method for creating a new Tag with auto-generated ID.
    /// </summary>
    public static Tag Create(string code, string displayName, string description = "")
        => new(TagId.CreateNew(), code, displayName, description);
}
