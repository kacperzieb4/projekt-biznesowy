using System;
using System.Collections.Generic;
using AttractionCatalog.Domain.Modules.CatalogSearch;
using Xunit;

namespace AttractionCatalog.Tests.Unit.Domain
{
    public class AvailabilityScheduleTests
    {
        [Fact]
        public void Schedule_Should_Allow_Available_Time()
        {
            // Arrange
            var schedule = new AvailabilitySchedule(100, new List<RuleId>());
            var compiler = new RuleSpecificationCompiler();
            var criteria = new SearchCriteria(new DateRange(DateTime.Now, DateTime.Now.AddDays(1)));

            // Act
            bool result = schedule.IsAvailable(DateTime.Now, compiler, new List<RuleDefinition>());

            // Assert
            Assert.True(result);
        }
    }
}
