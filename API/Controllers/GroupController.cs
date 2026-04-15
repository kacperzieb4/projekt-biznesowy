using AttractionCatalog.Application.Catalog.Features.Groups.Commands.CreateGroup;
using Microsoft.AspNetCore.Mvc;

namespace AttractionCatalog.API.Controllers;

public class GroupController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupCommand command)
    {
        var result = await Mediator.Send(command);
        return result.IsSuccess
            ? Created($"/api/group/{result.Value}", result.Value)
            : BadRequest(result.Error);
    }
}
