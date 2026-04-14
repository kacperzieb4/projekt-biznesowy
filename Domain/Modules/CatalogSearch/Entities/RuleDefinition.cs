using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;

namespace AttractionCatalog.Domain.Modules.CatalogSearch.Entities;

/// <summary>
/// A data-driven rule that controls availability of an attraction or scenario.
/// Rules are stored as structured data (not hardcoded logic) so they can be
/// modified at runtime without code changes.
/// </summary>
public class RuleDefinition
{
    public RuleId Id { get; }
    public RuleType Type { get; }
    public int Priority { get; }
    public Effect Effect { get; }
    public Dictionary<string, object> Params { get; }

    public RuleDefinition(RuleId id, RuleType type, int priority, Effect effect, Dictionary<string, object> parameters)
    {
        ArgumentNullException.ThrowIfNull(id);

        Id = id;
        Type = type;
        Priority = priority;
        Effect = effect;
        Params = parameters ?? new Dictionary<string, object>();
    }
}
