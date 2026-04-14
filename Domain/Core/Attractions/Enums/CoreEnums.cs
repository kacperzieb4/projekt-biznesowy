namespace AttractionCatalog.Domain.Core.Attractions.Enums;

/// <summary>
/// Lifecycle state of an attraction in the system.
/// </summary>
public enum AttractionState
{
    /// <summary>Being prepared, not yet visible to customers.</summary>
    Draft,

    /// <summary>Published and available for direct purchase.</summary>
    Catalog,

    /// <summary>Active but only sold as part of a group/package.</summary>
    Internal,

    /// <summary>Withdrawn from the offer.</summary>
    Archived
}

/// <summary>
/// Defines how components within a group must be visited.
/// </summary>
public enum SequenceMode
{
    /// <summary>Components must be visited in the defined order.</summary>
    Strict,

    /// <summary>Components can be visited in any order.</summary>
    Flexible,

    /// <summary>No ordering constraints.</summary>
    None
}

/// <summary>
/// Type of relationship between two attractions.
/// </summary>
public enum RelationType
{
    /// <summary>One attraction requires the other to be purchased/completed first.</summary>
    Requires,

    /// <summary>Two attractions cannot be combined together.</summary>
    Excludes,

    /// <summary>Soft suggestion — one attraction is recommended after the other.</summary>
    RecommendedAfter
}
