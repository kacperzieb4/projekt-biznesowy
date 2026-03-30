namespace Domain.Modules.CatalogSearch.Enums;

/// <summary>
/// The effect produced when a rule matches — either permits or denies access.
/// </summary>
public enum Effect
{
    /// <summary>Rule permits availability during the matched conditions.</summary>
    Allow = 0,

    /// <summary>Rule denies availability during the matched conditions.</summary>
    Deny = 1
}
