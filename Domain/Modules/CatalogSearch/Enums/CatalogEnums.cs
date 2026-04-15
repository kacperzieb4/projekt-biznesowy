namespace AttractionCatalog.Domain.Modules.CatalogSearch.Enums;

/// <summary>
/// Type of availability rule that controls when an attraction is open.
/// </summary>
public enum RuleType
{
    /// <summary>Recurring rule based on day of week (e.g. open Mon-Fri).</summary>
    Weekly,

    /// <summary>Date-range rule (e.g. summer season June-August).</summary>
    Seasonal,

    /// <summary>One-off override (e.g. closed on 1st of May).</summary>
    Exception
}

/// <summary>
/// Whether a rule allows or denies availability when it matches.
/// </summary>
public enum Effect
{
    Allow,
    Deny
}
