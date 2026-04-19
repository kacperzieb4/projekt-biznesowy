using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;
using Microsoft.Extensions.Hosting;

namespace AttractionCatalog.Infrastructure.DataSeeding;

public sealed class WieliczkaSeeder : IHostedService
{
    private readonly IAttractionRepository _repository;
    private readonly List<RuleDefinition> _globalRules;

    public WieliczkaSeeder(IAttractionRepository repository, List<RuleDefinition> globalRules)
    {
        _repository = repository;
        _globalRules = globalRules;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Kopalnia soli Wieliczka
        var historyTag = new Tag(new TagId(Guid.NewGuid()), "HISTORIA", "Historia", "Eksploracja dziejów, legendy i stare korytarze.");
        var miningTag = new Tag(new TagId(Guid.NewGuid()), "GORNICTWO", "Górnictwo", "Tradycyjne maszyny, szyby oraz metody wydobycia soli.");
        var sightseeingTag = new Tag(new TagId(Guid.NewGuid()), "ZWIEDZANIE", "Zwiedzanie", "Spokojny spacer z przewodnikiem po wytyczonych komorach.");
        var adventureTag = new Tag(new TagId(Guid.NewGuid()), "PRZYGODA", "Przygoda", "Wymaga zaangażowania fizycznego i wyjścia poza utarte turystyczne szlaki.");
        var familyTag = new Tag(new TagId(Guid.NewGuid()), "RODZINA", "Dla Rodzin", "Przestrzeń przyjazna i bezpieczna dla rodzin z młodszymi dziećmi.");
        var healthTag = new Tag(new TagId(Guid.NewGuid()), "ZDROWIE", "Odpoczynek i Zdrowie", "Korzystny mikroklimat solny nakierowany na regenerację.");


        // REGUŁY: Otwarcia w zależności od dnia (odwzorowane atrybutami)
        var touristRouteRule = new RuleDefinition(
            new RuleId(Guid.NewGuid()), RuleType.Weekly, priority: 15, Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom", 8 },
                { "TimeTo", 18 },
            });

        var minersRouteRule = new RuleDefinition(
            new RuleId(Guid.NewGuid()), RuleType.Weekly, priority: 15, Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom", 9 },
                { "TimeTo", 16 },
            });

        _globalRules.Add(touristRouteRule);
        _globalRules.Add(minersRouteRule);

        // HARMONOGRAMY
        var touristRouteSchedule = new AvailabilitySchedule(100, new List<RuleId> { touristRouteRule.Id });
        var minersRouteSchedule = new AvailabilitySchedule(100, new List<RuleId> { minersRouteRule.Id });
        var alwaysOpenSchedule = new AvailabilitySchedule(100, new List<RuleId>());

        // TRASY 
        var touristScenario = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Trasa Turystyczna", 
            TimeSpan.FromHours(2.5), // Czas trwania: 2-3h
            new List<Tag> { sightseeingTag, historyTag, familyTag, miningTag },
            touristRouteSchedule);

        var minersScenario = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Trasa Górnicza", 
            TimeSpan.FromHours(2.5), // Czas trwania: 2-3h
            new List<Tag> { miningTag, adventureTag, historyTag },
            minersRouteSchedule);

        var graduationTowerScenario = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Tężnia Solankowa",
            TimeSpan.FromHours(1),
            new List<Tag> { healthTag },
            alwaysOpenSchedule);

        var wieliczkaAttraction = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Kopalnia Soli \"Wieliczka\"",
            AttractionState.Catalog,
            new List<Tag>(),
            new Location(49.9833, 20.0500),
            alwaysOpenSchedule,
            new List<Scenario> { touristScenario, minersScenario });

        var graduationTowerAttraction = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Tężnia Solankowa",
            AttractionState.Catalog,
            new List<Tag> { healthTag },
            new Location(49.9833, 20.0500),
            alwaysOpenSchedule,
            new List<Scenario> { graduationTowerScenario });

        var wieliczkaGroup = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Wieliczka z Tężnią Solankową",
            SequenceMode.None,
            new List<Tag> { historyTag, healthTag },
            alwaysOpenSchedule,
            new List<IAttractionComponent> { wieliczkaAttraction, graduationTowerAttraction });

        await _repository.SaveAsync(wieliczkaAttraction, cancellationToken);
        await _repository.SaveAsync(graduationTowerAttraction, cancellationToken);
        await _repository.SaveAsync(wieliczkaGroup, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
