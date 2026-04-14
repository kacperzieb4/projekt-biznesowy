using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;
using AttractionCatalog.Domain.Modules.CatalogSearch.Services;
using Xunit;

namespace AttractionCatalog.Tests.Unit.Domain.Modules.CatalogSearch;

public class PriorityAvailabilityTests
{
    [Fact]
    public void High_Priority_Deny_Should_Override_Low_Priority_Allow()
    {
        // Arrange
        var compiler = new RuleSpecificationCompiler();
        var allowRuleId = new RuleId(Guid.NewGuid());
        var denyRuleId = new RuleId(Guid.NewGuid());

        var allowRule = new RuleDefinition(allowRuleId, RuleType.Weekly, 10, Effect.Allow, new());
        var denyRule = new RuleDefinition(denyRuleId, RuleType.Exception, 100, Effect.Deny, new());

        var schedule = new AvailabilitySchedule(0, [allowRuleId, denyRuleId]);
        var rules = new List<RuleDefinition> { allowRule, denyRule };

        // Act
        var isAvailable = schedule.IsAvailable(DateTime.Now, rules, compiler);

        // Assert — high priority DENY wins
        Assert.False(isAvailable);
    }
}
