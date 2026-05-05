using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Models.Dtos;
using WifiWarriorAPI.Models.Dtos.ConnectionInformations;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

/// <summary>
/// API endpoints for managing connection information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CanEdit")]
public class ConnectionInformationController : ControllerBase
{
    private readonly IConnectionInformationService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionInformationController"/> class.
    /// </summary>
    /// <param name="service">
    /// The service used to manage connection information operations.
    /// </param>
    public ConnectionInformationController(IConnectionInformationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all connection information records.
    /// </summary>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// An <see cref="OkObjectResult"/> containing the collection of connection information records.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<ConnectionInformationResponse>))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var rows = await _service.GetAllAsync(cancellationToken);
        return Ok(rows);
    }

    /// <summary>
    /// Retrieves a connection information record by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection information.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// An <see cref="OkObjectResult"/> containing the record when found; otherwise a <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConnectionInformationResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var row = await _service.GetByIdAsync(id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    /// <summary>
    /// Creates a new connection information record.
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
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ConnectionInformationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Post([FromBody] CreateConnectionInformationRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request, cancellationToken);

        if (result.Success && result.Value is not null)
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new ErrorResponse { Message = result.Error }),
            _ => Problem(result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Updates an existing connection information record.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection information.
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
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Put(long id, [FromBody] UpdateConnectionInformationRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);

        if (result.Success)
            return NoContent();

        return result.StatusCode switch
        {
            StatusCodes.Status404NotFound => NotFound(new ErrorResponse { Message = result.Error }),
            StatusCodes.Status400BadRequest => BadRequest(new ErrorResponse { Message = result.Error }),
            _ => Problem(result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Deletes a connection information record by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection information.
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
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);

        if (result.Success)
            return NoContent();

        return result.StatusCode switch
        {
            StatusCodes.Status404NotFound => NotFound(new ErrorResponse { Message = result.Error }),
            StatusCodes.Status400BadRequest => BadRequest(new ErrorResponse { Message = result.Error }),
            _ => Problem(result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }
}
