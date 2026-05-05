using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Accounts;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    /// <summary>
    /// Constructor for AccountController.
    /// </summary>
    /// <param name="accountService">The account service.</param>
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Registers a new user account and assigns the default <c>User</c> role.
    /// </summary>
    /// <param name="registerRequest">Registration request payload.</param>
    /// <param name="cancellationToken">The cancellation token for asynchronous operations.</param>
    /// <returns>
    /// <see cref="StatusCodes.Status201Created"/> if the user was registered successfully.
    /// <see cref="StatusCodes.Status400BadRequest"/> if the user was not registered successfully.
    /// <see cref="StatusCodes.Status500InternalServerError"/> if an error occurred during registration.
    /// </returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterAccountRequest registerRequest, CancellationToken cancellationToken)
    {
        var result = await _accountService.RegisterAsync(registerRequest, cancellationToken);
        
        if (result.Success)
            return StatusCode(StatusCodes.Status201Created, result.Value);

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            _ => Problem(detail: result.Error,
                statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginRequest">Login credentials.</param>
    /// <param name="cancellationToken">The cancellation token for asynchronous operations.</param>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/> if the user was authenticated successfully.
    /// <see cref="StatusCodes.Status401Unauthorized"/> if the user is not authorized to access the resource.
    /// <see cref="StatusCodes.Status500InternalServerError"/> if an error occurred during authentication.
    /// </returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginAccountRequest loginRequest, CancellationToken cancellationToken)
    {
        var result = await _accountService.LoginAsync(loginRequest, cancellationToken);
        
        if (result.Success)
            return Ok(result.Value);

        return result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(new { message = result.Error }),
            StatusCodes.Status401Unauthorized => Unauthorized(new { message = result.Error }),
            _ => Problem(detail: result.Error ?? "Unexpected error",
                statusCode: result.StatusCode ?? StatusCodes.Status500InternalServerError)
        };
    }
    
    /// <summary>
    /// Get the admin page.
    /// </summary>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/>
    /// </returns>
    [HttpGet]
    [Route("admin")]
    [Authorize(Roles = nameof(Role.Administrator))]
    public IActionResult GetAdmin()
    {
        return Ok("Admin Page");
    }
    
    /// <summary>
    /// Get the moderator page.
    /// </summary>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/>
    /// </returns>
    [HttpGet]
    [Route("moderator")]
    [Authorize(Roles = nameof(Role.Moderator))]
    public IActionResult GetMod()
    {
        return Ok("Moderator Page");
    }
    
    /// <summary>
    /// Get the user page.
    /// </summary>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/>
    /// </returns>
    [HttpGet]
    [Route("user")]
    [Authorize(Roles = nameof(Role.User))]
    public IActionResult GetUser()
    {
        return Ok("User Page");
    }
    
    /// <summary>
    /// Get the public page.
    /// </summary>
    /// <returns>
    /// <see cref="StatusCodes.Status200OK"/>
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("all")]
    public IActionResult GetPublic()
    {
        return Ok("Home Page");
    }
    
}
