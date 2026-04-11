using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Models.Dtos.Addresses;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

/// <summary>
/// API endpoints for managing addresses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CanEdit")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressController"/> class.
    /// </summary>
    /// <param name="addressService">
    /// The address service.
    /// </param>
    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }
    
    /// <summary>
    /// Retrieves all addresses.
    /// </summary>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// An <see cref="OkObjectResult"/> containing the collection of addresses.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<AddressResponse>))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var addresses = await _addressService.GetAllAddressesAsync(cancellationToken);
        return Ok(addresses);
    }
    
    /// <summary>
    /// Retrieves an address by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the address.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// An <see cref="OkObjectResult"/> containing the address when found; otherwise a <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddressResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var address = await _addressService.GetAddressByIdAsync(id, cancellationToken);
        return address is null ? NotFound() : Ok(address);
    }
    
    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="request">
    /// The create address request payload.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="CreatedAtActionResult"/> when the address is created successfully; otherwise an error response.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddressResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateAddressRequest request, CancellationToken cancellationToken)
    {
        var result = await _addressService.CreateAddressAsync(request, cancellationToken);

        if (result is { Success: true, Value: not null })
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            _ => Problem(detail: result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Updates an existing address.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the address.
    /// </param>
    /// <param name="request">
    /// The update address request payload.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="NoContentResult"/> when the address is updated successfully; otherwise an error response.
    /// </returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(long id, [FromBody] UpdateAddressRequest request, CancellationToken cancellationToken)
    {
        var result = await _addressService.UpdateAddressAsync(id, request, cancellationToken);

        if (result.Success)
            return NoContent();

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            StatusCodes.Status404NotFound => NotFound(new { message = result.Error }),
            _ => Problem(detail: result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Deletes an address by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the address.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="NoContentResult"/> when the address is deleted successfully; otherwise an error response.
    /// </returns>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _addressService.DeleteAddressAsync(id, cancellationToken);

        if (result.Success)
            return NoContent();

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            StatusCodes.Status404NotFound => NotFound(new { message = result.Error }),
            _ => Problem(detail: result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }
}