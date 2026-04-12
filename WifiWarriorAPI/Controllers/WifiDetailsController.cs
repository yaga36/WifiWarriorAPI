using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Models.Dtos.WifiDetails;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

/// <summary>
/// API endpoints for managing Wi-Fi login details.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CanEdit")]
public class WifiDetailsController : ControllerBase
{
    private readonly IWifiDetailsService _service;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="WifiDetailsController"/> class.
    /// </summary>
    /// <param name="service">
    /// The service used to manage Wi-Fi details operations.
    /// </param>
    public WifiDetailsController(IWifiDetailsService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Retrieves all Wi-Fi login details.
    /// </summary>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// An <see cref="OkObjectResult"/> containing the collection of Wi-Fi details.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<WifiDetailResponse>))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var rows = await _service.GetAllAsync(cancellationToken);
        return Ok(rows);
    }
    
    /// <summary>
    /// Retrieves Wi-Fi login details by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the Wi-Fi details.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// An <see cref="OkObjectResult"/> containing the Wi-Fi details when found; otherwise a <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WifiDetailResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var row = await _service.GetByIdAsync(id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    /// <summary>
    /// Creates new Wi-Fi login details.
    /// </summary>
    /// <param name="request">
    /// The create request payload.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="CreatedAtActionResult"/> when successful; otherwise an error response.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(WifiDetailResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateWifiDetailRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request, cancellationToken);

        if (result is { Success: true, Value: not null })
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            _ => Problem(result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Updates existing Wi-Fi login details.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the Wi-Fi details.
    /// </param>
    /// <param name="request">
    /// The update request payload.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="NoContentResult"/> when successful; otherwise an error response.
    /// </returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(long id, [FromBody] UpdateWifiDetailRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);

        if (result.Success)
            return NoContent();

        return result.StatusCode switch
        {
            StatusCodes.Status404NotFound => NotFound(new { message = result.Error }),
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            _ => Problem(result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Deletes Wi-Fi login details by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the Wi-Fi details.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="NoContentResult"/> when successful; otherwise an error response.
    /// </returns>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);

        if (result.Success)
            return NoContent();

        return result.StatusCode switch
        {
            StatusCodes.Status404NotFound => NotFound(new { message = result.Error }),
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            _ => Problem(result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }
}