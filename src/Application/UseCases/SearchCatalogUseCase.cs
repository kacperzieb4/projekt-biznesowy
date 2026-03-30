using Application.DTOs;
using Application.Interfaces;
using Domain.Core.Attractions.Entities;
using Domain.Core.Attractions.Interfaces;
using Domain.Core.Attractions.ValueObjects;
using Domain.Modules.CatalogSearch.Entities;
using Domain.Modules.CatalogSearch.Services;
using Domain.Modules.CatalogSearch.Specifications;
using Domain.Modules.CatalogSearch.ValueObjects;

namespace Application.UseCases;

/// <summary>
/// Use Case: Search the catalog for available attractions matching given criteria.
/// 
/// This is the main read-model use case (dedicated read path).
/// 
/// Flow:
/// 1. Map SearchQueryDto → domain SearchCriteria
/// 2. Build IQuerySpecification for pre-filtering (conditionally — only if geo filter present)
/// 3. Pre-filter via repository (database level)
/// 4. Delegate to CatalogSearchService for in-memory rule evaluation
/// 5. Map AvailabilityResult → CatalogDto (response projection)
/// </summary>
public sealed class SearchCatalogUseCase
{
    private readonly IAttractionRepository _repository;
    private readonly CatalogSearchService _searchService;
    private readonly Func<GeoArea, IQuerySpecification<IAttractionComponent>>? _locationSpecFactory;

    public SearchCatalogUseCase(
        IAttractionRepository repository,
        CatalogSearchService searchService,
        Func<GeoArea, IQuerySpecification<IAttractionComponent>>? locationSpecFactory = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        _locationSpecFactory = locationSpecFactory;
    }

    /// <summary>
    /// Executes the catalog search and returns projected DTOs.
    /// </summary>
    public async Task<IReadOnlyList<CatalogDto>> ExecuteAsync(
        SearchQueryDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // 1. Map DTO to domain SearchCriteria
        var criteria = MapToCriteria(request);

        // 2. Build pre-filter specification (conditionally)
        IQuerySpecification<IAttractionComponent>? preFilterSpec = null;
        if (criteria.HasGeoFilter && _locationSpecFactory is not null)
        {
            preFilterSpec = _locationSpecFactory(criteria.NearArea!);
        }

        // 3. Pre-filter at repository level
        var preFiltered = await _repository.FindByCriteriaAsync(preFilterSpec, cancellationToken);

        // 4. Domain-level evaluation
        var results = _searchService.FindAvailableAttractions(criteria, preFiltered);

        // 5. Map to response DTOs
        return MapToDto(results, preFiltered);
    }

    // ── Mapping Helpers ────────────────────────────────────────────────

    private static SearchCriteria MapToCriteria(SearchQueryDto dto)
    {
        var timeRange = new DateRange(dto.StartDate, dto.EndDate);

        GeoArea? geoArea = dto.HasGeoFilter
            ? new GeoArea(dto.CenterLatitude!.Value, dto.CenterLongitude!.Value, dto.RadiusKm!.Value)
            : null;

        TimeSpan? requiredDuration = dto.RequiredDurationMinutes.HasValue
            ? TimeSpan.FromMinutes(dto.RequiredDurationMinutes.Value)
            : null;

        var requiredTags = dto.RequiredTagIds.Select(g => new TagId(g)).ToList();
        var excludedTags = dto.ExcludedTagIds.Select(g => new TagId(g)).ToList();

        return new SearchCriteria(timeRange, geoArea, requiredDuration, requiredTags, excludedTags);
    }

    private static IReadOnlyList<CatalogDto> MapToDto(
        IReadOnlyList<AvailabilityResult> results,
        IReadOnlyList<IAttractionComponent> components)
    {
        var componentMap = components.ToDictionary(c => c.Id);

        return results
            .Where(r => r.IsAvailable)
            .Select(result =>
            {
                var component = componentMap.GetValueOrDefault(result.ComponentId);
                if (component is null) return null;

                var dto = new CatalogDto
                {
                    Id = result.ComponentId.Value,
                    Name = component.Name,
                    IsAvailable = result.IsAvailable,
                    Tags = component.Tags.Select(t => new CatalogTagDto
                    {
                        Code = t.Code,
                        DisplayName = t.DisplayName
                    }).ToList()
                };

                // Resolve available scenarios for SingleAttraction
                if (component is SingleAttraction single)
                {
                    var availableScenarioIds = result.AvailableScenarios.ToHashSet();
                    dto = dto with
                    {
                        AvailableScenarios = single.Scenarios
                            .Where(s => availableScenarioIds.Contains(s.Id))
                            .Select(s => new CatalogScenarioDto
                            {
                                Id = s.Id.Value,
                                Name = s.Name,
                                DurationMinutes = (int)s.Duration.TotalMinutes,
                                Tags = s.Tags.Select(t => new CatalogTagDto
                                {
                                    Code = t.Code,
                                    DisplayName = t.DisplayName
                                }).ToList()
                            }).ToList()
                    };
                }

                return dto;
            })
            .Where(dto => dto is not null)
            .ToList()
            .AsReadOnly()!;
    }
}
