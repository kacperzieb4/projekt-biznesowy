using Microsoft.AspNetCore.Mvc;
using TouristManagement.Application.UseCases;

namespace TouristManagement.Api.Controllers;

[ApiController]
[Route("api/attractions")]
public class AttractionController : ControllerBase
{
    private readonly CreateAttractionUseCase _createUseCase;
    public AttractionController(CreateAttractionUseCase createUseCase) => _createUseCase = createUseCase;

    [HttpPost]
    public IActionResult Post(CreateDto dto) => Ok(_createUseCase.Execute(dto));
}

[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly SearchCatalogUseCase _searchUseCase;
    public CatalogController(SearchCatalogUseCase searchUseCase) => _searchUseCase = searchUseCase;

    [HttpGet("search")]
    public IActionResult Search([FromQuery] DateTime date, [FromQuery] double? lat, [FromQuery] double? lon) 
        => Ok(_searchUseCase.Execute(new SearchQueryDto(date, lat, lon, 20)));
}