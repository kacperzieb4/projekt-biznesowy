using TouristManagement.Domain.Core.Attractions;
using TouristManagement.Domain.Modules.CatalogSearch;
using TouristManagement.Infrastructure.Persistence;

namespace TouristManagement.Application.UseCases;

public record CreateDto(string Name, double Lat, double Lon);
public record SearchQueryDto(DateTime Date, double? Lat, double? Lon, double? Radius);

public class CreateAttractionUseCase
{
    private readonly IAttractionRepository _repo;
    public CreateAttractionUseCase(IAttractionRepository repo) => _repo = repo;

    public Guid Execute(CreateDto request)
    {
        // 1. Tworzymy atrakcję
        var attr = new SingleAttraction(AttractionId.New(), request.Name, new Location(request.Lat, request.Lon));
        
        // 2. LOGIKA TESTOWA: Automatycznie dodajemy scenariusz, bo bez niego atrakcja jest "pusta"
        attr.AddScenario(new Scenario(ScenarioId.New(), "Standard Visit", TimeSpan.FromHours(2)));
        
        // 3. LOGIKA TESTOWA: Publikujemy od razu (zmieniamy DRAFT -> CATALOG)
        // W prawdziwym systemie robiłby to osobny przycisk "Publikuj"
        attr.Publish(); 

        _repo.Save(attr);
        return attr.Id.Value;
    }
}

public class SearchCatalogUseCase
{
    private readonly IAttractionRepository _repo;
    private readonly CatalogSearchService _searchService;

    public SearchCatalogUseCase(IAttractionRepository repo, CatalogSearchService searchService)
    {
        _repo = repo;
        _searchService = searchService;
    }

    public List<AvailabilityResult> Execute(SearchQueryDto query)
    {
        GeoArea? area = query.Lat.HasValue ? new GeoArea(query.Lat.Value, query.Lon.Value, query.Radius ?? 10) : null;
        var criteria = new SearchCriteria(new DateRange(query.Date, query.Date.AddHours(1)), area);

        List<IAttractionComponent> preFiltered;
        if (area != null) preFiltered = _repo.FindByCriteria(new LocationQuerySpecification(area));
        else preFiltered = _repo.GetAll();

        return _searchService.FindAvailableAttractions(criteria, preFiltered);
    }
}

public class AttractionGroupBuilder
{
    private string _name;
    private SequenceMode _mode;
    private List<IAttractionComponent> _components = new();

    public AttractionGroupBuilder WithName(string name) { _name = name; return this; }
    public AttractionGroupBuilder WithSequenceMode(SequenceMode mode) { _mode = mode; return this; }
    public AttractionGroupBuilder AddComponent(IAttractionComponent component)
    {
        if (component.State == AttractionState.Draft) throw new InvalidOperationException();
        _components.Add(component);
        return this;
    }
    public AttractionGroup Build() => new(AttractionId.New(), _name, _mode, _components);
}