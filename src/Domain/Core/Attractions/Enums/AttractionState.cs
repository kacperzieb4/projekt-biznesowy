namespace Domain.Core.Attractions.Enums;

/// <summary>
/// Lifecycle states for an attraction entity.
/// Transitions: DRAFT → CATALOG | INTERNAL → ARCHIVED
/// </summary>
public enum AttractionState
{
    /// <summary>Object under editorial preparation — not visible in catalog.</summary>
    Draft = 0,

    /// <summary>Verified and available for direct purchase in the catalog.</summary>
    Catalog = 1,

    /// <summary>Verified and operational, but sold only as part of groups/packages.</summary>
    Internal = 2,

    /// <summary>Withdrawn from the tourist offering; archived.</summary>
    Archived = 3
}
