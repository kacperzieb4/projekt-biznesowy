namespace AttractionCatalog.Domain.Core.Attractions.Enums
{
    public enum AttractionState { Draft, Catalog, Internal, Archived }
    public enum SequenceMode { STRICT, FLEXIBLE, NONE }
    public enum RelationType { Requires, Excludes, RecommendedAfter }
}
