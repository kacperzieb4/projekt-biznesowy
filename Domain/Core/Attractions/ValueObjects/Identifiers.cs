using System;

namespace AttractionCatalog.Domain.Core.Attractions.ValueObjects
{
    public record AttractionId(Guid Value);
    public record TagId(Guid Value);
    public record ScenarioId(Guid Value);
    public record RuleId(Guid Value);

    public record Location(double Latitude, double Longitude);
}
