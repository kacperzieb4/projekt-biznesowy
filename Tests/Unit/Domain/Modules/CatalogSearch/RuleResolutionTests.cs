using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;
using AttractionCatalog.Domain.Modules.CatalogSearch.Services;
using Xunit;

namespace AttractionCatalog.Tests.Unit.Domain.Modules.CatalogSearch;

public class RuleResolutionTests
{
    [Theory]
    [InlineData(0, 10, 100, false)]   // Entity(0) + Allow(10) < Entity(0) + Deny(100) → DENY wins
    [InlineData(100, 50, 10, true)]   // Entity(100) + Allow(50) > Entity(100) + Deny(10) → ALLOW wins
    [InlineData(0, 50, 50, true)]     // Same combined priority → first match (Allow) wins
    public void Priority_Resolution_Should_Respect_Combined_Priorities(
        int basePriority,
        int allowPriority,
        int denyPriority,
        bool expectedAvailability)
    {
        // Arrange
        var compiler = new RuleSpecificationCompiler();
        var allowId = new RuleId(Guid.NewGuid());
        var denyId = new RuleId(Guid.NewGuid());

        var allowRule = new RuleDefinition(allowId, RuleType.Weekly, allowPriority, Effect.Allow, new());
        var denyRule = new RuleDefinition(denyId, RuleType.Exception, denyPriority, Effect.Deny, new());

        var schedule = new AvailabilitySchedule(basePriority, [allowId, denyId]);
        var rules = new List<RuleDefinition> { allowRule, denyRule };

        // Act
        var result = schedule.IsAvailable(DateTime.Now, rules, compiler);

        // Assert
        Assert.Equal(expectedAvailability, result);
    }
}
