namespace Domain.Core.Attractions.Enums;

/// <summary>
/// Determines how components in an AttractionGroup must be sequenced.
/// </summary>
public enum SequenceMode
{
    /// <summary>Components must be visited in strict order.</summary>
    Strict = 0,

    /// <summary>Components can be visited in any order.</summary>
    Flexible = 1,

    /// <summary>No sequencing constraint.</summary>
    None = 2
}
