using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Services;
using Xunit;

namespace AttractionCatalog.Tests.Unit.Domain;

public class AvailabilityScheduleTests
{
    [Fact]
    public void Schedule_With_No_Rules_Should_Default_To_Available()
    {
        // Arrange
        var schedule = new AvailabilitySchedule(100, new List<RuleId>());
        var compiler = new RuleSpecificationCompiler();

        // Act
        var result = schedule.IsAvailable(DateTime.Now, new List<RuleDefinition>(), compiler);

        // Assert
        Assert.True(result);
    }
}
