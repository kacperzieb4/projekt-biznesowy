namespace Domain.Core.Attractions.Enums;

/// <summary>
/// Types of relationships between attractions.
/// </summary>
public enum RelationType
{
    /// <summary>Source attraction requires the target attraction.</summary>
    Requires = 0,

    /// <summary>Source attraction excludes the target attraction (mutually exclusive).</summary>
    Excludes = 1,

    /// <summary>Target attraction is recommended after the source.</summary>
    RecommendedAfter = 2
}
