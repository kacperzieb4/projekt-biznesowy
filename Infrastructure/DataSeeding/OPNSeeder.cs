using AttractionCatalog.Domain.Core.Attractions.Entities;
using AttractionCatalog.Domain.Core.Attractions.Enums;
using AttractionCatalog.Domain.Core.Attractions.Ports;
using AttractionCatalog.Domain.Core.Attractions.ValueObjects;
using AttractionCatalog.Domain.Modules.CatalogSearch.Entities;
using AttractionCatalog.Domain.Modules.CatalogSearch.Enums;
using Microsoft.Extensions.Hosting;

namespace AttractionCatalog.Infrastructure.DataSeeding;

public sealed class OPNSeeder : IHostedService
{
    private readonly IAttractionRepository _repository;
    private readonly List<RuleDefinition> _globalRules;

    public OPNSeeder(IAttractionRepository repository, List<RuleDefinition> globalRules)
    {
        _repository = repository;
        _globalRules = globalRules;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // =============================================================
        // REGUŁY DOSTĘPNOŚCI — Ojcowski Park Narodowy
        //
        // Sezon turystyczny: kwiecień (4) – październik (10)
        // Klucze Params: TimeFrom, TimeTo, MonthFrom, MonthTo
        // =============================================================

        // Ruiny zamków i Grodzisko: 09:00–18:00, sezon turystyczny
        var ruleSezon_9_18 = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Seasonal,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom",   9 },
                { "TimeTo",    18 },
                { "MonthFrom",  4 },  // kwiecień
                { "MonthTo",   10 },  // październik
            });

        // Jaskinia Łokietka i Zamek w Pieskowej Skale: 10:00–17:00, sezon turystyczny
        var ruleSezon_10_17 = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Seasonal,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom",  10 },
                { "TimeTo",   17 },
                { "MonthFrom", 4 },
                { "MonthTo",  10 },
            });

        // Jaskinia Ciemna — krótsze godziny ze względu na stopień trudności: 10:00–16:00
        var ruleSezon_10_16 = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Seasonal,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom",  10 },
                { "TimeTo",   16 },
                { "MonthFrom", 4 },
                { "MonthTo",  10 },
            });

        // Szlaki turystyczne: 06:00–20:00, sezon turystyczny
        var ruleSezonSzlaki = new RuleDefinition(
            new RuleId(Guid.NewGuid()),
            RuleType.Seasonal,
            priority: 10,
            Effect.Allow,
            new Dictionary<string, object>
            {
                { "TimeFrom",   6 },
                { "TimeTo",    20 },
                { "MonthFrom",  4 },
                { "MonthTo",   10 },
            });

        _globalRules.AddRange(new[] { ruleSezon_9_18, ruleSezon_10_17, ruleSezon_10_16, ruleSezonSzlaki });

        // =============================================================
        // HARMONOGRAMY
        // =============================================================
        var scheduleZamekOjcow     = new AvailabilitySchedule(100, new List<RuleId> { ruleSezon_9_18.Id });
        var scheduleJaskiniaLok    = new AvailabilitySchedule(100, new List<RuleId> { ruleSezon_10_17.Id });
        var scheduleJaskiniaCiemna = new AvailabilitySchedule(100, new List<RuleId> { ruleSezon_10_16.Id });
        var schedulePieskowaSkala  = new AvailabilitySchedule(100, new List<RuleId> { ruleSezon_10_17.Id });
        var scheduleSzlaki         = new AvailabilitySchedule(100, new List<RuleId> { ruleSezonSzlaki.Id });
        var scheduleGrodzisko      = new AvailabilitySchedule(100, new List<RuleId> { ruleSezon_9_18.Id });
        var zawszeOtwarty          = new AvailabilitySchedule(100, new List<RuleId>());

        // =============================================================
        // TAGI
        // =============================================================
        var tagHistoria    = new Tag(new TagId(Guid.NewGuid()), "HISTORIA",    "Historia i kultura",      "Zabytki historyczne i dziedzictwo kulturowe");
        var tagPrzyroda    = new Tag(new TagId(Guid.NewGuid()), "PRZYRODA",    "Przyroda OPN",            "Naturalne atrakcje przyrodnicze parku");
        var tagWidoki      = new Tag(new TagId(Guid.NewGuid()), "WIDOKI",      "Punkty widokowe",         "Panoramy i punkty obserwacyjne");
        var tagSzlak       = new Tag(new TagId(Guid.NewGuid()), "SZLAK",       "Szlaki turystyczne",      "Piesze szlaki turystyczne OPN");
        var tagJaskinia    = new Tag(new TagId(Guid.NewGuid()), "JASKINIA",    "Jaskinie",                "Podziemne atrakcje jaskiniowe");
        var tagZamek       = new Tag(new TagId(Guid.NewGuid()), "ZAMEK",       "Zamki jurajskie",         "Zamki i ruiny jurajskie Wyżyny Krakowsko-Częstochowskiej");
        var tagSakralne    = new Tag(new TagId(Guid.NewGuid()), "SAKRALNE",    "Obiekty sakralne",        "Kaplice i miejsca kultu religijnego");
        var tagTechnika    = new Tag(new TagId(Guid.NewGuid()), "TECHNIKA",    "Zabytki techniki",        "Historyczne obiekty przemysłowe i techniczne");
        var tagArcheologia = new Tag(new TagId(Guid.NewGuid()), "ARCHEOLOGIA", "Stanowiska archeologiczne","Miejsca badań prehistorycznych i neandertalskich");
        var tagRodziny     = new Tag(new TagId(Guid.NewGuid()), "RODZINY",     "Przyjazny rodzinom",      "Trasy dostępne dla rodzin z wózkami dziecięcymi");

        // =============================================================
        // 1. ZAMEK KAZIMIERZOWSKI W OJCOWIE
        //    a = Zwiedzanie ruin       (09–18, sezon)
        //    b = Wycieczka z przewod.  (09–18, sezon)
        //    c = Ruiny + Ogród Ojcowski(09–18, sezon)
        // =============================================================
        var scenZamekRuiny = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Zwiedzanie ruin zamku",
            TimeSpan.FromHours(1.5), new List<Tag> { tagHistoria, tagWidoki, tagZamek },
            scheduleZamekOjcow);

        var scenZamekPrzewodnik = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Zwiedzanie z przewodnikiem",
            TimeSpan.FromHours(2), new List<Tag> { tagHistoria, tagZamek },
            scheduleZamekOjcow);

        var scenZamekOgrod = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Ruiny zamku + Ogród Ojcowski",
            TimeSpan.FromHours(2.5), new List<Tag> { tagHistoria, tagWidoki, tagZamek, tagPrzyroda },
            scheduleZamekOjcow);

        var zamekKazimierzowski = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Zamek Kazimierzowski w Ojcowie",
            AttractionState.Catalog,
            new List<Tag> { tagHistoria, tagWidoki, tagZamek },
            new Location(50.2095, 19.8222),
            zawszeOtwarty,
            new List<Scenario>
            {
                scenZamekRuiny,       // a      09–18
                scenZamekPrzewodnik,  // b      09–18
                scenZamekOgrod,       // a+c    09–18
            });

        // =============================================================
        // 2. JASKINIA ŁOKIETKA
        //    a = Wycieczka z przewodnikiem (10–17, sezon)
        //    Temperatura stała 7–8°C, parking w Czajowicach (300 m)
        // =============================================================
        var scenLokietka = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Wycieczka z przewodnikiem",
            TimeSpan.FromHours(1), new List<Tag> { tagJaskinia, tagHistoria },
            scheduleJaskiniaLok);

        var jaskiniaLokietka = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Jaskinia Łokietka",
            AttractionState.Catalog,
            new List<Tag> { tagJaskinia, tagHistoria },
            new Location(50.2170, 19.8365),
            zawszeOtwarty,
            new List<Scenario> { scenLokietka });

        // =============================================================
        // 3. JASKINIA CIEMNA
        //    a = Wycieczka z przewodnikiem (10–16, sezon)
        //    Brak oświetlenia elektrycznego; obozowisko neandertalczyków ~50 tys. lat p.n.e.
        // =============================================================
        var scenCiemnaStandard = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Wycieczka z przewodnikiem",
            TimeSpan.FromMinutes(45), new List<Tag> { tagJaskinia, tagArcheologia, tagPrzyroda },
            scheduleJaskiniaCiemna);

        var jaskiniaCiemna = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Jaskinia Ciemna",
            AttractionState.Catalog,
            new List<Tag> { tagJaskinia, tagArcheologia },
            new Location(50.2082, 19.8218),
            zawszeOtwarty,
            new List<Scenario> { scenCiemnaStandard });

        // =============================================================
        // 4. BRAMA KRAKOWSKA, JONASZÓWKA I ŹRÓDŁO MIŁOŚCI
        //    Rejon płaski i utwardzony — najbardziej przyjazny rodzinom.
        //    Zawsze dostępny (teren zewnętrzny, brak biletów).
        //    a = Spacer przy Bramie Krakowskiej
        //    b = Panorama z Jonaszówki
        //    c = Źródło Miłości
        //    a+b+c = Pełny spacer
        // =============================================================
        var scenBramaKrakowska = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Spacer przy Bramie Krakowskiej",
            TimeSpan.FromMinutes(30), new List<Tag> { tagPrzyroda, tagWidoki, tagRodziny },
            zawszeOtwarty);

        var scenJonaszowka = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Panorama z Jonaszówki",
            TimeSpan.FromMinutes(45), new List<Tag> { tagWidoki },
            zawszeOtwarty);

        var scenZrodloMilosci = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Źródło Miłości",
            TimeSpan.FromMinutes(20), new List<Tag> { tagPrzyroda },
            zawszeOtwarty);

        var scenBramaFull = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Brama Krakowska + Jonaszówka + Źródło Miłości",
            TimeSpan.FromHours(1.5), new List<Tag> { tagPrzyroda, tagWidoki, tagRodziny },
            zawszeOtwarty);

        var bramaKrakowskaOkolice = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Brama Krakowska, Jonaszówka i Źródło Miłości",
            AttractionState.Catalog,
            new List<Tag> { tagPrzyroda, tagWidoki, tagRodziny },
            new Location(50.2085, 19.8220),
            zawszeOtwarty,
            new List<Scenario>
            {
                scenBramaKrakowska,  // a
                scenJonaszowka,      // b
                scenZrodloMilosci,   // c
                scenBramaFull,       // a+b+c
            });

        // =============================================================
        // 5. ZAMEK W PIESKOWEJ SKALE I MACZUGA HERKULESA
        //    a = Zwiedzanie muzeum (10–17, sezon)
        //    b = Maczuga Herkulesa i okolice (zawsze, teren zewn.)
        //    a+b = Zamek + Maczuga
        // =============================================================
        var scenMuzeum = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Zwiedzanie muzeum zamkowego",
            TimeSpan.FromHours(2), new List<Tag> { tagHistoria, tagZamek },
            schedulePieskowaSkala);

        var scenMaczuga = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Maczuga Herkulesa i okolice",
            TimeSpan.FromHours(1), new List<Tag> { tagWidoki, tagPrzyroda },
            zawszeOtwarty);

        var scenZamekMaczuga = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Muzeum zamkowe + Maczuga Herkulesa",
            TimeSpan.FromHours(3), new List<Tag> { tagHistoria, tagZamek, tagWidoki, tagPrzyroda },
            schedulePieskowaSkala);  // muzeum ogranicza kombo

        var zamekPieskowaSkala = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Zamek w Pieskowej Skale i Maczuga Herkulesa",
            AttractionState.Catalog,
            new List<Tag> { tagHistoria, tagZamek, tagWidoki },
            new Location(50.2320, 19.8460),
            zawszeOtwarty,
            new List<Scenario>
            {
                scenMuzeum,        // a      10–17
                scenMaczuga,       // b      zawsze
                scenZamekMaczuga,  // a+b    10–17 (muzeum ogranicza)
            });

        // =============================================================
        // 6. SZLAK ORLICH GNIAZD (CZERWONY) — 13,6 km
        //    Jedyny szlak rekomendowany dla wózków (odcinek Ojców–Brama Krakowska).
        //    a = Pełny szlak (ok. 4 h)
        //    b = Odcinek dostępny Ojców–Brama Krakowska (ok. 2 h)
        // =============================================================
        var scenSzlakCzerwonyFull = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Pełny Szlak Orlich Gniazd (13,6 km)",
            TimeSpan.FromHours(4), new List<Tag> { tagSzlak, tagWidoki, tagPrzyroda },
            scheduleSzlaki);

        var scenSzlakCzerwonyOdcinek = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Odcinek Ojców–Brama Krakowska",
            TimeSpan.FromHours(2), new List<Tag> { tagSzlak, tagPrzyroda, tagRodziny },
            scheduleSzlaki);

        var szlakCzerwony = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Szlak Orlich Gniazd (Czerwony)",
            AttractionState.Catalog,
            new List<Tag> { tagSzlak, tagWidoki, tagPrzyroda },
            new Location(50.2095, 19.8225),
            zawszeOtwarty,
            new List<Scenario>
            {
                scenSzlakCzerwonyFull,     // a   pełny
                scenSzlakCzerwonyOdcinek,  // b   odcinek rodzinny
            });

        // =============================================================
        // 7. SZLAK WAROWNI JURAJSKICH (NIEBIESKI) — Wąwóz Ciasne Skałki
        //    Duże nachylenie; wymaga obuwia z profilowanym bieżnikiem.
        //    Łączy Jaskinię Łokietka z Bramą Krakowską.
        // =============================================================
        var scenSzlakNiebieski = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Wąwóz Ciasne Skałki",
            TimeSpan.FromHours(2), new List<Tag> { tagSzlak, tagPrzyroda },
            scheduleSzlaki);

        var szlakNiebieski = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Szlak Warowni Jurajskich (Niebieski)",
            AttractionState.Catalog,
            new List<Tag> { tagSzlak, tagPrzyroda },
            new Location(50.2175, 19.8360),
            zawszeOtwarty,
            new List<Scenario> { scenSzlakNiebieski });

        // =============================================================
        // 8. SZLAK PARK ZAMKOWY – JASKINIA CIEMNA (ZIELONY)
        //    Trasa górska przez Górę Koronną; ekspozycja widokowa.
        //    Odradzana dla rodzin z wózkami i osób z lękiem wysokości.
        // =============================================================
        var scenSzlakZielony = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Góra Koronna – trasa widokowa",
            TimeSpan.FromHours(2.5), new List<Tag> { tagSzlak, tagWidoki },
            scheduleSzlaki);

        var szlakZielony = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Szlak Park Zamkowy – Jaskinia Ciemna (Zielony)",
            AttractionState.Catalog,
            new List<Tag> { tagSzlak, tagWidoki },
            new Location(50.2090, 19.8220),
            zawszeOtwarty,
            new List<Scenario> { scenSzlakZielony });

        // =============================================================
        // 9. SZLAK DOLINY SĄSPOWSKIEJ (ŻÓŁTY)
        //    Najlepszy na ucieczkę od tłumów; teren może być podmokły.
        // =============================================================
        var scenSzlakZolty = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Spacer Doliną Sąspowską",
            TimeSpan.FromHours(2), new List<Tag> { tagSzlak, tagPrzyroda },
            scheduleSzlaki);

        var szlakZolty = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Szlak Doliny Sąspowskiej (Żółty)",
            AttractionState.Catalog,
            new List<Tag> { tagSzlak, tagPrzyroda },
            new Location(50.2060, 19.8100),
            zawszeOtwarty,
            new List<Scenario> { scenSzlakZolty });

        // =============================================================
        // 10. SZLAK CZARNY (SKAŁA JONASZÓWKA)
        //     Łącznik widokowy; kluczowy punkt — Skała Jonaszówka.
        // =============================================================
        var scenSzlakCzarny = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Punkt widokowy Skała Jonaszówka",
            TimeSpan.FromHours(1), new List<Tag> { tagSzlak, tagWidoki },
            scheduleSzlaki);

        var szlakCzarny = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Szlak Czarny – Skała Jonaszówka",
            AttractionState.Catalog,
            new List<Tag> { tagSzlak, tagWidoki },
            new Location(50.2090, 19.8235),
            zawszeOtwarty,
            new List<Scenario> { scenSzlakCzarny });

        // =============================================================
        // 11. KAPLICA „NA WODZIE"
        //     Unikalna konstrukcja na palach; widoczna ze szlaku czerwonego.
        //     Teren zewnętrzny — zawsze dostępna.
        // =============================================================
        var scenKaplica = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Kaplica na palach – wizyta",
            TimeSpan.FromMinutes(30), new List<Tag> { tagSakralne },
            zawszeOtwarty);

        var kaplicaNaWodzie = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Kaplica „Na Wodzie\"",
            AttractionState.Catalog,
            new List<Tag> { tagSakralne },
            new Location(50.2100, 19.8242),
            zawszeOtwarty,
            new List<Scenario> { scenKaplica });

        // =============================================================
        // 12. GRODZISKO
        //     Kompleks Salomei z rzeźbą słonia. Miejsce wyciszenia z dala
        //     od głównego potoku turystów.
        // =============================================================
        var scenGrodzisko = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Kompleks Salomei i rzeźba słonia",
            TimeSpan.FromHours(1), new List<Tag> { tagSakralne, tagHistoria },
            scheduleGrodzisko);

        var grodzisko = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Grodzisko",
            AttractionState.Catalog,
            new List<Tag> { tagSakralne, tagHistoria },
            new Location(50.2048, 19.8148),
            zawszeOtwarty,
            new List<Scenario> { scenGrodzisko });

        // =============================================================
        // 13. MŁYN BORONIA (INTERNAL)
        //     Najlepiej zachowany zabytek techniki OPN.
        //     Wnętrza dostępne WYŁĄCZNIE po telefonicznym umówieniu —
        //     modelowany jako INTERNAL (sprzedawany tylko w pakietach).
        // =============================================================
        var scenMlyn = new Scenario(
            new ScenarioId(Guid.NewGuid()), "Zwiedzanie wnętrza Młyna Boronia",
            TimeSpan.FromHours(1), new List<Tag> { tagTechnika, tagHistoria },
            zawszeOtwarty);

        var mlynBoronia = new SingleAttraction(
            new AttractionId(Guid.NewGuid()),
            "Młyn Boronia",
            AttractionState.Internal,  // tylko po wcześniejszym umówieniu — sprzedaż wyłącznie w pakietach
            new List<Tag> { tagTechnika, tagHistoria },
            new Location(50.2110, 19.8268),
            zawszeOtwarty,
            new List<Scenario> { scenMlyn });

        // =============================================================
        // GRUPY ATRAKCJI (pakiety turystyczne OPN)
        // =============================================================

        // Dzień w Ojcowie: Zamek Kazimierzowski + Jaskinia Łokietka + Brama Krakowska
        var dzienWOjcowie = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Dzień w Ojcowie",
            SequenceMode.Flexible,
            new List<Tag> { tagHistoria, tagJaskinia, tagZamek, tagWidoki, tagPrzyroda, tagRodziny },
            zawszeOtwarty,
            new List<IAttractionComponent> { zamekKazimierzowski, jaskiniaLokietka, bramaKrakowskaOkolice });

        // Odkrywca OPN: Jaskinia Ciemna + Szlak Zielony + Grodzisko
        var odkrywcaOPN = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Odkrywca OPN",
            SequenceMode.Flexible,
            new List<Tag> { tagJaskinia, tagArcheologia, tagSzlak, tagWidoki, tagSakralne, tagHistoria },
            zawszeOtwarty,
            new List<IAttractionComponent> { jaskiniaCiemna, szlakZielony, grodzisko });

        // Szlak Zamków Jurajskich: Zamek Kazimierzowski + Zamek Pieskowa Skała + Szlak Czerwony
        var szlakZamkow = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Szlak Zamków Jurajskich",
            SequenceMode.Strict,
            new List<Tag> { tagHistoria, tagZamek, tagSzlak, tagWidoki },
            zawszeOtwarty,
            new List<IAttractionComponent> { zamekKazimierzowski, szlakCzerwony, zamekPieskowaSkala });

        // Tajemnice OPN: Jaskinia Łokietka + Jaskinia Ciemna + Grodzisko + Młyn Boronia (INTERNAL)
        var tajemniceOPN = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Tajemnice OPN",
            SequenceMode.Flexible,
            new List<Tag> { tagJaskinia, tagHistoria, tagArcheologia, tagSakralne, tagTechnika },
            zawszeOtwarty,
            new List<IAttractionComponent> { jaskiniaLokietka, jaskiniaCiemna, grodzisko, mlynBoronia });

        // Pełny OPN: flagowy pakiet obejmujący wszystkie główne punkty
        var pelnyOPN = new AttractionGroup(
            new AttractionId(Guid.NewGuid()),
            "Pełny OPN",
            SequenceMode.None,
            new List<Tag>
            {
                tagHistoria, tagPrzyroda, tagWidoki, tagSzlak, tagJaskinia,
                tagZamek, tagSakralne, tagTechnika, tagArcheologia, tagRodziny
            },
            zawszeOtwarty,
            new List<IAttractionComponent>
            {
                zamekKazimierzowski,
                jaskiniaLokietka,
                jaskiniaCiemna,
                bramaKrakowskaOkolice,
                zamekPieskowaSkala,
                szlakCzerwony,
                kaplicaNaWodzie,
                grodzisko,
                mlynBoronia,
            });

        // =============================================================
        // ZAPIS DO REPOZYTORIUM
        // =============================================================
        await _repository.SaveAsync(zamekKazimierzowski,    cancellationToken);
        await _repository.SaveAsync(jaskiniaLokietka,       cancellationToken);
        await _repository.SaveAsync(jaskiniaCiemna,         cancellationToken);
        await _repository.SaveAsync(bramaKrakowskaOkolice,  cancellationToken);
        await _repository.SaveAsync(zamekPieskowaSkala,     cancellationToken);
        await _repository.SaveAsync(szlakCzerwony,          cancellationToken);
        await _repository.SaveAsync(szlakNiebieski,         cancellationToken);
        await _repository.SaveAsync(szlakZielony,           cancellationToken);
        await _repository.SaveAsync(szlakZolty,             cancellationToken);
        await _repository.SaveAsync(szlakCzarny,            cancellationToken);
        await _repository.SaveAsync(kaplicaNaWodzie,        cancellationToken);
        await _repository.SaveAsync(grodzisko,              cancellationToken);
        await _repository.SaveAsync(mlynBoronia,            cancellationToken);

        await _repository.SaveAsync(dzienWOjcowie,   cancellationToken);
        await _repository.SaveAsync(odkrywcaOPN,     cancellationToken);
        await _repository.SaveAsync(szlakZamkow,     cancellationToken);
        await _repository.SaveAsync(tajemniceOPN,    cancellationToken);
        await _repository.SaveAsync(pelnyOPN,        cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
