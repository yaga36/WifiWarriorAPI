using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Models.Dtos.Venues;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

/// <summary>
/// API endpoints for managing venues.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VenueController : ControllerBase
{
    private readonly IVenueService _venueService;

    /// <summary>
    /// Initializes a new instance of the <see cref="VenueController"/> class.
    /// </summary>
    /// <param name="venueService">
    /// The venue service.
    /// </param>
    public VenueController(IVenueService venueService)
    {
        _venueService = venueService;
    }
    
    /// <summary>
    /// Retrieves all venues.
    /// </summary>
    /// <returns>A <see cref="OkObjectResult"/> containing the venue collection.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<VenueResponse>))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var venues = await _venueService.GetAllVenuesAsync(cancellationToken);
        return Ok(venues);
    }

    /// <summary>
    /// Retrieves all full venue setups.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>An <see cref="OkObjectResult"/> containing the full venue setup collection.</returns>
    [HttpGet("full")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<VenueSetupResponse>))]
    public async Task<IActionResult> GetFull(CancellationToken cancellationToken)
    {
        var venueSetups = await _venueService.GetAllVenueSetupsAsync(cancellationToken);
        return Ok(venueSetups);
    }

    /// <summary>
    /// Retrieves a venue by identifier.
    /// </summary>
    /// <param name="id">The venue identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="OkObjectResult"/> when found; otherwise <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VenueResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var venue = await _venueService.GetVenueByIdAsync(id, cancellationToken);
        return venue is null ? NotFound() : Ok(venue);
    }

    /// <summary>
    /// Retrieves a full venue setup by venue identifier.
    /// </summary>
    /// <param name="id">The venue identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="OkObjectResult"/> when found; otherwise <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpGet("full/{id:long}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VenueSetupResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFullById(long id, CancellationToken cancellationToken)
    {
        var venueSetup = await _venueService.GetVenueSetupByIdAsync(id, cancellationToken);
        return venueSetup is null ? NotFound() : Ok(venueSetup);
    }

    /// <summary>
    /// Creates a new venue.
    /// </summary>
    /// <param name="venueRequest">The create venue request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A <see cref="CreatedAtActionResult"/> for the created venue.</returns>
    [HttpPost]
    [Authorize(Policy = "CanSubmit")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VenueResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post([FromBody] CreateVenueRequest venueRequest, CancellationToken cancellationToken)
    {
        var createdVenue = await _venueService.CreateVenueAsync(venueRequest, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { createdVenue.Id }, createdVenue);
    }

    /// <summary>
    /// Creates a full venue setup in a single request.
    /// </summary>
    /// <param name="request">The aggregate venue setup request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A <see cref="CreatedAtActionResult"/> when successful; otherwise an error response.</returns>
    [HttpPost("full")]
    [Authorize(Policy = "CanSubmit")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VenueSetupResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PostFull([FromBody] CreateVenueSetupRequest request, CancellationToken cancellationToken)
    {
        var result = await _venueService.CreateVenueSetupAsync(request, cancellationToken);

        if (result is { Success: true, Value: not null })
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Venue.Id }, result.Value);

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            _ => Problem(detail: result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Updates an existing venue.
    /// </summary>
    /// <param name="id">The venue identifier.</param>
    /// <param name="venueRequest">The update venue request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="NoContentResult"/> when updated; otherwise <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpPut("{id:long}")]
    [Authorize(Policy = "CanEdit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put(long id, [FromBody] UpdateVenueRequest venueRequest, CancellationToken cancellationToken)
    {
        var updatedVenue = await _venueService.UpdateVenueAsync(id, venueRequest, cancellationToken);
        return updatedVenue ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a venue by identifier.
    /// </summary>
    /// <param name="id">The venue identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="NoContentResult"/> when deleted; otherwise <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deletedVenue = await _venueService.DeleteVenueAsync(id, cancellationToken);
        return deletedVenue ? NoContent() : NotFound();
    }
}