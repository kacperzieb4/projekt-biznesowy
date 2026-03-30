using TouristManagement.Domain.Core.Attractions;

namespace TouristManagement.Domain.Modules.CatalogSearch;

public interface ISpecification<T> { bool IsSatisfiedBy(T candidate); }

public class RuleSpecificationCompiler
{
    public ISpecification<SearchCriteria> Compile(List<RuleDefinition> rules)
    {
        // Logika kompilacji reguł do specyfikacji w pamięci
        return new DummyCriteriaSpec(); 
    }
    private class DummyCriteriaSpec : ISpecification<SearchCriteria> { 
        public bool IsSatisfiedBy(SearchCriteria c) => true; 
    }
}

public class CatalogSearchService
{
    public List<AvailabilityResult> FindAvailableAttractions(SearchCriteria criteria, List<IAttractionComponent> preFiltered)
    {
        var results = new List<AvailabilityResult>();

        foreach (var component in preFiltered)
        {
            if (component.State != AttractionState.Catalog) continue;

            bool isAvailable = false;
            var availableScenarios = new List<ScenarioId>();

            if (component is SingleAttraction single)
            {
                // Przecięcie (Intersection): Reguły Globalne AND Reguły Scenariusza
                foreach (var scenario in single.Scenarios)
                {
                    bool globalOk = single.Schedule.IsAvailable(criteria.TimeRange.Start);
                    bool scenarioOk = scenario.Schedule.IsAvailable(criteria.TimeRange.Start);
                    
                    if (globalOk && scenarioOk) availableScenarios.Add(scenario.Id);
                }
                isAvailable = availableScenarios.Any();
            }
            else if (component is AttractionGroup group)
            {
                // Logika Composite: Reguły grupy AND dostępność wszystkich dzieci
                bool groupRulesOk = group.Schedule.IsAvailable(criteria.TimeRange.Start);
                bool childrenOk = group.Children.All(c => c.Schedule.IsAvailable(criteria.TimeRange.Start));
                isAvailable = groupRulesOk && childrenOk;
            }

            if (isAvailable)
                results.Add(new AvailabilityResult(component.Id, true, availableScenarios, new()));
        }

        return results;
    }
}