using Application.DTOs;
using Application.UseCases;
using Domain.Core.Attractions.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// REST Controller for managing tourist attractions.
/// Thin layer — delegates all logic to Application Use Cases.
/// 
/// Endpoints:
///   POST /api/attractions         → Create a new attraction (Draft)
///   PUT  /api/attractions/{id}/publish  → Publish attraction (Draft → Catalog)
///   GET  /api/attractions/{id}    → Get attraction by ID
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AttractionController : ControllerBase
{
    private readonly CreateAttractionUseCase _createUseCase;
    private readonly PublishAttractionUseCase _publishUseCase;

    public AttractionController(
        CreateAttractionUseCase createUseCase,
        PublishAttractionUseCase publishUseCase)
    {
        _createUseCase = createUseCase ?? throw new ArgumentNullException(nameof(createUseCase));
        _publishUseCase = publishUseCase ?? throw new ArgumentNullException(nameof(publishUseCase));
    }

    /// <summary>
    /// POST /api/attractions — Creates a new tourist attraction in Draft state.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AttractionCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromBody] CreateAttractionDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var attractionId = await _createUseCase.ExecuteAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = attractionId.Value },
                new AttractionCreatedResponse { Id = attractionId.Value });
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
    }

    /// <summary>
    /// PUT /api/attractions/{id}/publish — Publishes a Draft attraction to Catalog.
    /// </summary>
    [HttpPut("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var attractionId = new AttractionId(id);
            await _publishUseCase.ExecuteAsync(attractionId, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
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

    /// <summary>
    /// GET /api/attractions/{id} — Retrieves an attraction by ID (placeholder).
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(Guid id)
    {
        // Placeholder — full implementation would use a dedicated query/read use case
        return Ok(new { Id = id, Message = "Detailed attraction read not implemented in Phase 1." });
    }
}

/// <summary>API response for attraction creation.</summary>
public sealed record AttractionCreatedResponse
{
    public Guid Id { get; init; }
}
