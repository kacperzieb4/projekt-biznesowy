using System.Collections.Generic;
using System.Linq;
using AttractionCatalog.Application.Catalog.DTOs;
using AttractionCatalog.Domain.Modules.CatalogSearch.ValueObjects;

namespace AttractionCatalog.Application.Catalog.Features.Catalog.Queries.SearchCatalog
{
    public static class SearchCatalogMapper
    {
        public static List<CatalogDto> MapToDto(this List<AvailabilityResult> results)
        {
            return results.Select(r => new CatalogDto
            {
                Id = r.ComponentId.Value,
                Name = "Attraction (Mapped)",
                IsAvailable = r.IsAvailable
            }).ToList();
        }
    }
}
