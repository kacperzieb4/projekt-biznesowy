using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;
using Microsoft.Extensions.Hosting;

namespace AttractionCatalog.Infrastructure.DataSeeding;

public sealed class OperaKrakowskaSeeder : IHostedService
{
    private readonly IAttractionRepository _repository;
    private readonly List<RuleDefinition> _globalRules;

    public OperaKrakowskaSeeder(IAttractionRepository repository, List<RuleDefinition> globalRules)
    {
        _repository = repository;
        _globalRules = globalRules;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        

        // Spektakle wieczorne: wt–nd 18:00–23:00
        var ruleSpektakl = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Weekly,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom", 18 },
                { "TimeTo",   23 },
            });

        // Wycieczka za kulisy i warsztaty: pn–pt 10:00–14:00
        var ruleDzien = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Weekly,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom", 10 },
                { "TimeTo",   14 },
            });

        // Kolacja przedspektaklowa: 17:00–19:00 cały sezon
        var ruleKolacja = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Weekly,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom", 17 },
                { "TimeTo",   19 },
            });

        // Bar w foyer: 17:30–23:00 w dni spektakli
        var ruleFoyer = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Weekly,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom", 17 },
                { "TimeTo",   23 },
            });

        // Opera na Dziedzińcu: lipiec–sierpień 20:00–23:00
        var ruleDziedziniec = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Seasonal,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom",   20 },
                { "TimeTo",     23 },
                { "MonthFrom",   7 },
                { "MonthTo",     8 },
            });

        _globalRules.AddRange(new[]
        {
            ruleSpektakl, ruleDzien, ruleKolacja, ruleFoyer, ruleDziedziniec
        });

        var scheduleSpektakl = new AvailabilitySchedule(100, new List<RuleId> { ruleSpektakl.Id });
        var scheduleDzien = new AvailabilitySchedule(100, new List<RuleId> { ruleDzien.Id });
        var scheduleKolacja = new AvailabilitySchedule(100, new List<RuleId> { ruleKolacja.Id });
        var scheduleFoyer = new AvailabilitySchedule(100, new List<RuleId> { ruleFoyer.Id });
        var scheduleDziedziniec = new AvailabilitySchedule(100, new List<RuleId> { ruleDziedziniec.Id });
        var zawszeOtwarty = new AvailabilitySchedule(100, new List<RuleId>());

        var tagOpera = new Tag(new TagId(Guid.NewGuid()), "OPERA", "Opera", "Spektakle operowe");
        var tagBalet = new Tag(new TagId(Guid.NewGuid()), "BALET", "Balet", "Spektakle baletowe");
        var tagOperetka = new Tag(new TagId(Guid.NewGuid()), "OPERETKA", "Operetka", "Spektakle operetkowe i repertuar familijny");
        var tagKulisy = new Tag(new TagId(Guid.NewGuid()), "KULISY", "Za kulisami", "Zwiedzanie kulis i warsztaty edukacyjne");
        var tagGastronomia = new Tag(new TagId(Guid.NewGuid()), "GASTRO", "Gastronomia", "Kolacja i bar w foyer");
        var tagPlener = new Tag(new TagId(Guid.NewGuid()), "PLENER", "Plener", "Spektakle plenerowe na dziedzińcu");
        var tagPremium = new Tag(new TagId(Guid.NewGuid()), "PREMIUM", "Premium", "Pakiety premium z kolacją i barem");
        var tagEdukacja = new Tag(new TagId(Guid.NewGuid()), "EDUKACJA", "Edukacja", "Warsztaty wokalne i zwiedzanie kulis");
        var tagRodzinny = new Tag(new TagId(Guid.NewGuid()), "RODZINNY", "Rodzinny", "Spektakle i atrakcje dla rodzin z dziećmi");

        var operaLoc = new Location(50.0647, 19.9502);

        // =============================================================
        // 1. SCENA GŁÓWNA
        //    a = Spektakl operowy   (scheduleSpektakl : 18–23)
        //    b = Spektakl baletowy  (scheduleSpektakl : 18–23)
        //    c = Spektakl operetkowy(scheduleSpektakl : 17–23 przez ruleFoyer jako scenariusz familijny)
        //
        //    Spektakle wzajemnie się wykluczają (jedna scena, jeden wieczór),
        //    dlatego nie ma scenariuszy combo między nimi.
        // =============================================================

        var scenOpera = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Spektakl Operowy",
            TimeSpan.FromMinutes(150), new List<Tag> { tagOpera },
            scheduleSpektakl);                      // 18–23

        var scenBalet = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Spektakl Baletowy",
            TimeSpan.FromMinutes(135), new List<Tag> { tagBalet },
            scheduleSpektakl);                      // 18–23

        var scenOperetka = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Spektakl Operetkowy",
            TimeSpan.FromMinutes(120), new List<Tag> { tagOperetka, tagRodzinny },
            scheduleFoyer);                         // 17–23 (wcześniejszy start dla rodzin)

        var scenaGlowna = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Opera Krakowska — Scena Główna",
            AttractionState.Catalog,
            new List<Tag> { tagOpera, tagBalet, tagOperetka },
            operaLoc,
            zawszeOtwarty,
            new List<Scenario>
            {
                scenOpera,    // 18–23
                scenBalet,    // 18–23
                scenOperetka, // 17–23
            });

        // =============================================================
        // 2. STREFA EDUKACYJNA
        //    a = Wycieczka za kulisy  (scheduleDzien : 10–14)
        //    b = Warsztaty wokalne    (scheduleDzien : 10–14, max 10 osób)
        //    a+b = Dzień edukacyjny   (scheduleDzien : 10–14)
        // =============================================================

        var scenKulisy = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Wycieczka za kulisy",
            TimeSpan.FromMinutes(90), new List<Tag> { tagKulisy, tagEdukacja },
            scheduleDzien);                         // 10–14

        var scenWarsztaty = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Warsztaty wokalne z solistą",
            TimeSpan.FromMinutes(60), new List<Tag> { tagEdukacja },
            scheduleDzien);                         // 10–14

        var scenDzienEdukacyjny = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Warsztaty + Kulisy",
            TimeSpan.FromMinutes(150), new List<Tag> { tagKulisy, tagEdukacja },
            scheduleDzien);                         // 10–14 (kulisy ograniczają combo)

        var strefaEdukacyjna = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Opera Krakowska — Strefa Edukacyjna",
            AttractionState.Catalog,
            new List<Tag> { tagKulisy, tagEdukacja },
            operaLoc,
            zawszeOtwarty,
            new List<Scenario>
            {
                scenKulisy,          // 10–14
                scenWarsztaty,       // 10–14
                scenDzienEdukacyjny, // 10–14
            });

        // =============================================================
        // 3. GASTRONOMIA
        //    a = Kolacja przedspektaklowa (scheduleKolacja : 17–19)
        //    b = Bar w foyer             (scheduleFoyer   : 17:30–23)
        //    a+b = Kolacja + Bar          (scheduleKolacja : 17–19, kolacja ogranicza start)
        // =============================================================

        var scenKolacja = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Kolacja przedspektaklowa",
            TimeSpan.FromMinutes(120), new List<Tag> { tagGastronomia },
            scheduleKolacja);                       // 17–19

        var scenFoyer = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Bar w foyer",
            TimeSpan.FromMinutes(30), new List<Tag> { tagGastronomia },
            scheduleFoyer);                         // 17–23 (przerwa spektaklu)

        var scenKolacjaFoyer = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Kolacja + Bar w foyer",
            TimeSpan.FromMinutes(150), new List<Tag> { tagGastronomia, tagPremium },
            scheduleKolacja);                       // 17–19 start (kolacja ogranicza)

        var gastronomia = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Opera Krakowska — Gastronomia",
            AttractionState.Catalog,
            new List<Tag> { tagGastronomia },
            operaLoc,
            zawszeOtwarty,
            new List<Scenario>
            {
                scenKolacja,      // 17–19
                scenFoyer,        // 17–23
                scenKolacjaFoyer, // 17–19
            });

        // =============================================================
        // 4. OPERA NA DZIEDZIŃCU (sezonowa, lipiec–sierpień)
        //    Jeden scenariusz — plenerowy spektakl letni 20:00–23:00
        // =============================================================

        var scenDziedziniec = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Letni spektakl plenerowy",
            TimeSpan.FromMinutes(150), new List<Tag> { tagOpera, tagPlener },
            scheduleDziedziniec);                   // 20–23, lipiec–sierpień

        var operaDziedziniec = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Opera na Dziedzińcu",
            AttractionState.Catalog,
            new List<Tag> { tagOpera, tagPlener, tagRodzinny },
            operaLoc,
            scheduleDziedziniec,
            new List<Scenario>
            {
                scenDziedziniec, // 20–23, lato
            });

      //pakiety
        // Wieczór Premium: Kolacja + Scena Główna + Bar w foyer
        var wieczorowPremium = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Wieczór Premium",
            SequenceMode.None,
            new List<Tag> { tagOpera, tagGastronomia, tagPremium },
            zawszeOtwarty,
            new List<IAttractionComponent> { gastronomia, scenaGlowna });

        // Dzień z Operą: Strefa Edukacyjna + Scena Główna
        var dzienZOpera = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Dzień z Operą",
            SequenceMode.None,
            new List<Tag> { tagEdukacja, tagKulisy, tagOpera },
            zawszeOtwarty,
            new List<IAttractionComponent> { strefaEdukacyjna, scenaGlowna });

        await _repository.SaveAsync(scenaGlowna, cancellationToken);
        await _repository.SaveAsync(strefaEdukacyjna, cancellationToken);
        await _repository.SaveAsync(gastronomia, cancellationToken);
        await _repository.SaveAsync(operaDziedziniec, cancellationToken);
        await _repository.SaveAsync(wieczorowPremium, cancellationToken);
        await _repository.SaveAsync(dzienZOpera, cancellationToken);
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}