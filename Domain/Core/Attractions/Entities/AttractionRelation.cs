using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;

namespace AttractionCatalog.Domain.Core.Attractions.Entities
{
    public class AttractionRelation
    {
        public AttractionId From { get; }
        public AttractionId To { get; }
        public RelationType Type { get; }

        public AttractionRelation(AttractionId from, AttractionId to, RelationType type)
        {
            From = from;
            To = to;
            Type = type;
        }
    }
}
