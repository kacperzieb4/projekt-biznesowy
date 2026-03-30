namespace Domain.Modules.CatalogSearch.Enums;

/// <summary>
/// Classification of availability rules.
/// </summary>
public enum RuleType
{
    /// <summary>Recurring weekly pattern (e.g., open Mon-Fri).</summary>
    Weekly = 0,

    /// <summary>Seasonal availability (e.g., summer hours).</summary>
    Seasonal = 1,

    /// <summary>One-off exception (e.g., holiday closure, special event).</summary>
    Exception = 2
}
