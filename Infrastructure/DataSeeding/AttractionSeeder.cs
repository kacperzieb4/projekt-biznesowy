using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;
using Microsoft.Extensions.Hosting;

namespace AttractionCatalog.Infrastructure.DataSeeding;

public sealed class AttractionSeeder : IHostedService
{
    private readonly IAttractionRepository _repository;
    private readonly List<RuleDefinition> _globalRules;

    // List<RuleDefinition> to globalny singleton zarejestrowany w Application/DependencyInjection.cs.
    // Seeder dodaje do niego reguły, które później CatalogSearchService odczytuje przy ewaluacji.
    public AttractionSeeder(IAttractionRepository repository, List<RuleDefinition> globalRules)
    {
        _repository = repository;
        _globalRules = globalRules;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // =============================================================
        // REGUŁY DOSTĘPNOŚCI
        //
        // Klucze w Params (konwencja do implementacji IsSatisfiedBy):
        //   TimeFrom  — godzina otwarcia (int, 0-23)
        //   TimeTo    — godzina zamknięcia (int, 0-23)
        //   MonthFrom — miesiąc początku sezonu (int, 1-12)  [tylko Seasonal]
        //   MonthTo   — miesiąc końca sezonu (int, 1-12)     [tylko Seasonal]
        //
        // Priority — lokalny priorytet reguły (BasePriority harmonogramu + Priority reguły = łączny)
        // Effect.Allow — w tym przedziale atrakcja jest dostępna
        // =============================================================

        // Godziny strefy basenowej i saun: 08:00 – 22:00, cały rok
        var ruleBasen = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Weekly,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom", 8 },
                { "TimeTo",  22 },
            });

        // Godziny siłowni: 08:00 – 23:00, cały rok
        var ruleSilownia = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Weekly,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom", 8 },
                { "TimeTo",  23 },
            });

        // Godziny śniadań: 07:00 – 11:00, cały rok
        var ruleSniadanie = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Weekly,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom",  7 },
                { "TimeTo",   11 },
            });

        // Strefa parkowa: 12:00 – 20:00, tylko lato (czerwiec–sierpień)
        // Łączy ograniczenie godzinowe i sezonowe w jednej regule Seasonal.
        var rulePark = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Seasonal,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom",  12 },
                { "TimeTo",    20 },
                { "MonthFrom",  6 },  // czerwiec
                { "MonthTo",    8 },  // sierpień
            });

        // Rejestrujemy wszystkie reguły w globalnej liście (odczytuje ją CatalogSearchService)
        _globalRules.AddRange(new[] { ruleBasen, ruleSilownia, ruleSniadanie, rulePark });

        // =============================================================
        // HARMONOGRAMY (AvailabilitySchedule)
        // BasePriority = 100 → efektywny priorytet reguły = 100 + Priority reguły
        //
        //   scheduleBasen    — strefa basenowa i sauny (08–22)
        //   scheduleSilownia — siłownia (08–23)
        //   scheduleSniadanie— śniadania (07–11)
        //   schedulePark     — strefa parkowa (12–20, lato)
        //   zawszeOtwarty    — brak reguł = domyślnie dostępny (nocleg hotelowy)
        // =============================================================
        var scheduleBasen     = new AvailabilitySchedule(100, new List<RuleId> { ruleBasen.Id });
        var scheduleSilownia  = new AvailabilitySchedule(100, new List<RuleId> { ruleSilownia.Id });
        var scheduleSniadanie = new AvailabilitySchedule(100, new List<RuleId> { ruleSniadanie.Id });
        var schedulePark      = new AvailabilitySchedule(100, new List<RuleId> { rulePark.Id });
        var zawszeOtwarty     = new AvailabilitySchedule(100, new List<RuleId>());

        // =============================================================
        // TAGI
        // =============================================================
        var tagWoda     = new Tag(new TagId(Guid.NewGuid()), "WODA",     "Atrakcje wodne",  "Baseny i atrakcje wodne");
        var tagSauna    = new Tag(new TagId(Guid.NewGuid()), "SAUNA",    "Strefa saun",     "Sauny suche, parowe i łaźnie");
        var tagPark     = new Tag(new TagId(Guid.NewGuid()), "PARK",     "Strefa parkowa",  "Zjeżdżalnie, jacuzzi i atrakcje zewnętrzne");
        var tagFitness  = new Tag(new TagId(Guid.NewGuid()), "FITNESS",  "Strefa fitness",  "Siłownia i ćwiczenia");
        var tagGrupy    = new Tag(new TagId(Guid.NewGuid()), "GRUPY",    "Zajęcia grupowe", "Zumba, yoga, aerobik i inne zajęcia grupowe");
        var tagNocleg   = new Tag(new TagId(Guid.NewGuid()), "NOCLEG",   "Nocleg w hotelu", "Zakwaterowanie hotelowe");
        var tagJedzenie = new Tag(new TagId(Guid.NewGuid()), "JEDZENIE", "Wyżywienie",      "Śniadania i inne posiłki");

        // =============================================================
        // 1. KOMPLEKS BASENOWY
        //    a = Hala basenowa (scheduleBasen  : 08–22)
        //    b = Sauny         (scheduleBasen  : 08–22)
        //    c = Strefa park.  (schedulePark   : 12–20, lato)
        //
        //    Scenariusze combo dziedziczą harmonogram BARDZIEJ restrykcyjnej strefy.
        //    np. a+c → schedulePark, bo park ogranicza kiedy obie są dostępne razem.
        // =============================================================
        var scenBasen = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Hala basenowa",
            TimeSpan.FromHours(2), new List<Tag> { tagWoda },
            scheduleBasen);                         // 08–22

        var scenSauny = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Sauny",
            TimeSpan.FromHours(2), new List<Tag> { tagSauna },
            scheduleBasen);                         // 08–22

        var scenPark = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Strefa parkowa",
            TimeSpan.FromHours(2), new List<Tag> { tagPark },
            schedulePark);                          // 12–20, tylko lato

        var scenBasenSauny = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Hala basenowa + Sauny",
            TimeSpan.FromHours(3), new List<Tag> { tagWoda, tagSauna },
            scheduleBasen);                         // 08–22 (park nie wchodzi)

        var scenBasenPark = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Hala basenowa + Strefa parkowa",
            TimeSpan.FromHours(3), new List<Tag> { tagWoda, tagPark },
            schedulePark);                          // 12–20, lato (park ogranicza)

        var scenSaunyPark = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Sauny + Strefa parkowa",
            TimeSpan.FromHours(3), new List<Tag> { tagSauna, tagPark },
            schedulePark);                          // 12–20, lato

        var scenBasenFull = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Hala basenowa + Sauny + Strefa parkowa",
            TimeSpan.FromHours(4), new List<Tag> { tagWoda, tagSauna, tagPark },
            schedulePark);                          // 12–20, lato (park ogranicza całość)

        var kompleksBasenowy = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Kompleks Basenowy",
            AttractionState.Catalog,
            new List<Tag> { tagWoda, tagSauna, tagPark },
            new Location(52.2297, 21.0122),
            zawszeOtwarty,                          // brak globalnego blokowania na poziomie atrakcji
            new List<Scenario>
            {
                scenBasen,       // a        08–22
                scenSauny,       // b        08–22
                scenPark,        // c        12–20 lato
                scenBasenSauny,  // a+b      08–22
                scenBasenPark,   // a+c      12–20 lato
                scenSaunyPark,   // b+c      12–20 lato
                scenBasenFull,   // a+b+c    12–20 lato
            });

        // =============================================================
        // 2. FITPARK
        //    a = Siłownia        (scheduleSilownia : 08–23)
        //    b = Zajęcia grupowe (scheduleBasen    : 08–22, jak cała strefa)
        // =============================================================
        var scenSilownia = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Siłownia",
            TimeSpan.FromHours(1.5), new List<Tag> { tagFitness },
            scheduleSilownia);                      // 08–23

        var scenZajecia = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Zajęcia grupowe",
            TimeSpan.FromHours(1), new List<Tag> { tagGrupy },
            scheduleBasen);                         // 08–22 (zajęcia kończą się wcześniej)

        var scenFitparkFull = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Siłownia + Zajęcia grupowe",
            TimeSpan.FromHours(2.5), new List<Tag> { tagFitness, tagGrupy },
            scheduleBasen);                         // 08–22 (zajęcia ograniczają combo)

        var fitpark = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Fitpark",
            AttractionState.Catalog,
            new List<Tag> { tagFitness, tagGrupy },
            new Location(52.2298, 21.0125),
            zawszeOtwarty,
            new List<Scenario>
            {
                scenSilownia,    // a     08–23
                scenZajecia,     // b     08–22
                scenFitparkFull, // a+b   08–22
            });

        // =============================================================
        // 3. HOTEL
        //    a = Nocleg     (zawszeOtwarty  : całodobowy)
        //    b = Śniadanie  (scheduleSniadanie : 07–11)
        // =============================================================
        var scenNocleg = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Nocleg",
            TimeSpan.FromHours(12), new List<Tag> { tagNocleg },
            zawszeOtwarty);                         // całodobowy

        var scenSniadanie = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Śniadanie",
            TimeSpan.FromHours(1), new List<Tag> { tagJedzenie },
            scheduleSniadanie);                     // 07–11

        var scenBB = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Nocleg + Śniadanie (B&B)",
            TimeSpan.FromHours(13), new List<Tag> { tagNocleg, tagJedzenie },
            scheduleSniadanie);                     // B&B — harmonogram ogranicza śniadanie (07–11)

        var hotel = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Hotel",
            AttractionState.Catalog,
            new List<Tag> { tagNocleg, tagJedzenie },
            new Location(52.2295, 21.0120),
            zawszeOtwarty,
            new List<Scenario>
            {
                scenNocleg,    // a     całodobowy
                scenSniadanie, // b     07–11
                scenBB,        // a+b   07–11
            });

        // =============================================================
        // GRUPY ATRAKCJI (pakiety sprzedażowe)
        // Grupy nie dodają własnych reguł godzinowych —
        // ich dzieci (SingleAttraction + Scenariusze) już to egzekwują.
        // =============================================================

        // Dzień Aktywny: Basen (pełen) + Fitpark (pełen). Bez noclegu.
        var dzienAktywny = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Dzień Aktywny",
            SequenceMode.None,
            new List<Tag> { tagWoda, tagSauna, tagPark, tagFitness, tagGrupy },
            zawszeOtwarty,
            new List<IAttractionComponent> { kompleksBasenowy, fitpark });

        // Dzień Wellness: Basen + Fitpark (lżejsza wersja dla relaksu).
        var dzienWellness = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Dzień Wellness",
            SequenceMode.None,
            new List<Tag> { tagWoda, tagSauna, tagGrupy },
            zawszeOtwarty,
            new List<IAttractionComponent> { kompleksBasenowy, fitpark });

        // Weekend Relaks: Hotel B&B + Kompleks Basenowy.
        var weekendRelaks = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Weekend Relaks",
            SequenceMode.None,
            new List<Tag> { tagNocleg, tagJedzenie, tagWoda, tagSauna, tagPark },
            zawszeOtwarty,
            new List<IAttractionComponent> { hotel, kompleksBasenowy });

        // Weekend Premium: Hotel B&B + Basen + Fitpark. Flagowy pakiet "wszystko w cenie".
        var weekendPremium = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Weekend Premium",
            SequenceMode.None,
            new List<Tag> { tagNocleg, tagJedzenie, tagWoda, tagSauna, tagPark, tagFitness, tagGrupy },
            zawszeOtwarty,
            new List<IAttractionComponent> { hotel, kompleksBasenowy, fitpark });

        // =============================================================
        // ZAPIS DO REPOZYTORIUM
        // Reguły są już w _globalRules powyżej (AddRange). Teraz zapisujemy atrakcje.
        // =============================================================
        await _repository.SaveAsync(kompleksBasenowy, cancellationToken);
        await _repository.SaveAsync(fitpark, cancellationToken);
        await _repository.SaveAsync(hotel, cancellationToken);

        await _repository.SaveAsync(dzienAktywny, cancellationToken);
        await _repository.SaveAsync(dzienWellness, cancellationToken);
        await _repository.SaveAsync(weekendRelaks, cancellationToken);
        await _repository.SaveAsync(weekendPremium, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
