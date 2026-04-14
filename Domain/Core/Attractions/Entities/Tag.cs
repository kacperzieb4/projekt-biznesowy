using AttractionCatalog.Domain.Core.Attractions.ValueObjects;

namespace AttractionCatalog.Domain.Core.Attractions.Entities;

/// <summary>
/// A label that categorizes attractions (e.g. "Family-Friendly", "Audio-Guide").
/// Tags drive the SearchCriteria filtering logic.
/// </summary>
public class Tag
{
    public TagId Id { get; }
    public string Code { get; }
    public string DisplayName { get; }
    public string Description { get; }

    public Tag(TagId id, string code, string displayName, string description)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Id = id;
        Code = code;
        DisplayName = displayName;
        Description = description;
    }
}
