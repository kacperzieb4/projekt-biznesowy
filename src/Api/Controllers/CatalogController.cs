using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// REST Controller for catalog search operations.
/// Exposes the search/filtering capabilities of the CatalogSearchService
/// via a parameterized GET endpoint.
/// 
/// Endpoints:
///   GET /api/catalog/search — Search for available attractions
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class CatalogController : ControllerBase
{
    private readonly SearchCatalogUseCase _searchUseCase;

    public CatalogController(SearchCatalogUseCase searchUseCase)
    {
        _searchUseCase = searchUseCase ?? throw new ArgumentNullException(nameof(searchUseCase));
    }

    /// <summary>
    /// GET /api/catalog/search — Searches for available attractions matching criteria.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<CatalogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] SearchQueryDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var results = await _searchUseCase.ExecuteAsync(request, cancellationToken);
            return Ok(results);
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
            return BadRequest(new ProblemDetails
            {
                Title = "Search Error",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
