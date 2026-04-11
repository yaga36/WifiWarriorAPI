using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Models.Dtos.ConnectionTypes;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

/// <summary>
/// API endpoints for managing connection types.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CanEdit")]
public class ConnectionTypesController : ControllerBase
{
    private readonly IConnectionTypeService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionTypesController"/> class.
    /// </summary>
    /// <param name="service">
    /// The service used to manage connection type operations.
    /// </param>
    public ConnectionTypesController(IConnectionTypeService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Retrieves all connection types.
    /// </summary>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// An <see cref="OkObjectResult"/> containing the collection of connection types.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<ConnectionTypeResponse>))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var connectionTypes = await _service.GetAllAsync(cancellationToken);
        return Ok(connectionTypes);
    }
    
    /// <summary>
    /// Retrieves a connection type by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection type.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// An <see cref="OkObjectResult"/> containing the connection type when found; otherwise a <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConnectionTypeResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var connectionType = await _service.GetByIdAsync(id, cancellationToken);
        return connectionType is null ? NotFound() : Ok(connectionType);
    }
    
    /// <summary>
    /// Creates a new connection type.
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
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ConnectionTypeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateConnectionTypeRequest request, CancellationToken cancellationToken)
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
    /// Updates an existing connection type.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection type.
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
    public async Task<IActionResult> Put(long id, [FromBody] UpdateConnectionTypeRequest request, CancellationToken cancellationToken)
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
    /// Deletes a connection type by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection type.
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
