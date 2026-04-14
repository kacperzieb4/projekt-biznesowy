using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;
using AttractionCatalog.Domain.Modules.CatalogSearch.Services;
using Xunit;

namespace AttractionCatalog.Tests.Unit.Domain.Core.Attractions;

public class RecursiveAvailabilityTests
{
    [Fact]
    public void Group_Should_Be_Unavailable_When_Any_Child_Is_Blocked()
    {
        // Arrange — a deep hierarchy with one blocked child (Composite Pattern)
        var compiler = new RuleSpecificationCompiler();
        var allowRuleId = new RuleId(Guid.NewGuid());
        var denyRuleId = new RuleId(Guid.NewGuid());

        var rules = new List<RuleDefinition>
        {
            new(allowRuleId, RuleType.Weekly, 10, Effect.Allow, new()),
            new(denyRuleId, RuleType.Exception, 100, Effect.Deny, new())
        };

        var availableSchedule = new AvailabilitySchedule(0, [allowRuleId]);
        var blockedSchedule = new AvailabilitySchedule(0, [denyRuleId]);

        var child1 = new SingleAttraction(
            new AttractionId(Guid.NewGuid()), "Child 1", AttractionState.Catalog,
            [], new Location(0, 0), availableSchedule, []);

        var child2 = new SingleAttraction(
            new AttractionId(Guid.NewGuid()), "Child 2", AttractionState.Catalog,
            [], new Location(0, 0), blockedSchedule, []);

        var group = new AttractionGroup(
            new AttractionId(Guid.NewGuid()), "Root Group", SequenceMode.Flexible,
            [], availableSchedule, [child1, child2]);

        var searchService = new CatalogSearchService(compiler, rules);

        // Act
        var result = searchService.CheckAvailability(group, DateTime.Now);

        // Assert — the entire group is unavailable because Child 2 is blocked
        Assert.False(result);
    }
}
