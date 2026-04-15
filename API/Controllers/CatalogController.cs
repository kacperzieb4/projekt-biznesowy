using AttractionCatalog.Application.Catalog.DTOs;
using AttractionCatalog.Application.Catalog.Features.Catalog.Queries.SearchCatalog;
using Microsoft.AspNetCore.Mvc;

namespace AttractionCatalog.API.Controllers;

public class CatalogController : ApiControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<List<CatalogDto>>> Search([FromQuery] SearchCatalogQuery query)
    {
        var result = await Mediator.Send(query);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}
