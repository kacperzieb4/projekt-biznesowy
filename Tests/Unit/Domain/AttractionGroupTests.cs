using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using Xunit;

namespace AttractionCatalog.Tests.Unit.Domain;

public class AttractionGroupTests
{
    [Fact]
    public void Group_With_Multiple_Components_Should_Aggregate_Logic()
    {
        // Arrange
        var schedule = new AvailabilitySchedule(0, new List<RuleId>());
        var child = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Muzeum",
            AttractionState.Catalog,
            [],
            new Location(0, 0),
            schedule,
            []);

        var components = new List<IAttractionComponent> { child };

        // Act
        var group = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Pass",
            SequenceMode.Flexible,
            [],
            schedule,
            components);

        // Assert
        Assert.Single(group.Components);
    }
}
