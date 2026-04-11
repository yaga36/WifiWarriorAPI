using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Models.Dtos.Users;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

/// <summary>
/// API endpoints for managing users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CanEdit")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="userService">
    /// The user service.
    /// </param>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A <see cref="OkObjectResult"/> containing the user collection.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<UserResponse>))]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }
    
    /// <summary>
    /// Retrieves a user by identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="OkObjectResult"/> when found; otherwise <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The create user request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="CreatedAtActionResult"/> when created;
    /// otherwise <see cref="BadRequestObjectResult"/> or <see cref="ConflictObjectResult"/>.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateUserAsync(request, cancellationToken);

        if (result is { Success: true, Value: not null })
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            StatusCodes.Status409Conflict => Conflict(new { message = result.Error }),
            _ => Problem(detail: result.Error, statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="request">The update user request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="NoContentResult"/> when updated;
    /// otherwise <see cref="BadRequestObjectResult"/> or <see cref="NotFoundObjectResult"/>.
    /// </returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateUserAsync(id, request, cancellationToken);

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
    /// Deletes a user by identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="NoContentResult"/> when deleted;
    /// otherwise <see cref="BadRequestObjectResult"/> or <see cref="NotFoundObjectResult"/>.
    /// </returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "CanDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUserAsync(id, cancellationToken);

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