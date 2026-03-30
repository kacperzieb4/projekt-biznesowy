using Application.DTOs;
using Application.UseCases;
using Domain.Core.Attractions.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// REST Controller for managing attraction groups (packages).
/// 
/// Endpoints:
///   POST /api/groups — Create a new attraction group
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class GroupController : ControllerBase
{
    private readonly CreateAttractionGroupUseCase _createGroupUseCase;

    public GroupController(CreateAttractionGroupUseCase createGroupUseCase)
    {
        _createGroupUseCase = createGroupUseCase ?? throw new ArgumentNullException(nameof(createGroupUseCase));
    }

    /// <summary>
    /// POST /api/groups — Creates a new attraction group from existing attractions.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GroupCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post(
        [FromBody] CreateGroupDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var groupId = await _createGroupUseCase.ExecuteAsync(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new GroupCreatedResponse
            {
                Id = groupId.Value
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Business Rule Violation",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }
}

/// <summary>API response for group creation.</summary>
public sealed record GroupCreatedResponse
{
    public Guid Id { get; init; }
}
