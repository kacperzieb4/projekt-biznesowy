using AttractionCatalog.Domain.Core.Attractions.ValueObjects;

namespace AttractionCatalog.Domain.Core.Attractions.Entities
{
    public class Tag
    {
        public TagId Id { get; }
        public string Code { get; }
        public string DisplayName { get; }
        public string Description { get; }

        public Tag(TagId id, string code, string displayName, string description)
        {
            Id = id;
            Code = code;
            DisplayName = displayName;
            Description = description;
        }
    }
}
