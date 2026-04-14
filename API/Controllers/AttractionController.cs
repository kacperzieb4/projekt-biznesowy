using AttractionCatalog.Application.Catalog.Features.Attractions.Commands.CreateAttraction;
using AttractionCatalog.Application.Catalog.Features.Attractions.Commands.PublishAttraction;
using AttractionCatalog.Application.Catalog.Features.Attractions.Queries.GetAttraction;
using Microsoft.AspNetCore.Mvc;

namespace AttractionCatalog.API.Controllers;

public class AttractionController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttractionCommand command)
    {
        var result = await Mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetAttractionQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var result = await Mediator.Send(new PublishAttractionCommand(id));
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
