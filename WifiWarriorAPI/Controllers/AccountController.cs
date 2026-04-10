using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<Users> _userManager;
    private readonly IAuthManager _authManager;
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Constructor for AccountController.
    /// </summary>
    /// <param name="userManager">User manager.</param>
    /// <param name="authManager">Authentication manager.</param>
    /// <param name="logger">Logger.</param>
    public AccountController(UserManager<Users> userManager, IAuthManager authManager, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _authManager = authManager;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user account and assigns the default <c>User</c> role.
    /// </summary>
    /// <param name="user">Registration payload.</param>
    /// <returns>
    /// <see cref="StatusCodes.Status202Accepted"/> if the user was registered successfully.
    /// <see cref="StatusCodes.Status400BadRequest"/> if the user was not registered successfully.
    /// <see cref="StatusCodes.Status500InternalServerError"/> if an error occurred during registration.
    /// </returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserInfo user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userIdentity = new Users
            {
                UserName = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                NormalizedEmail = user.Email,
                PhoneNumber = user.PhoneNumber ?? "",
            };

            var createResult = await _userManager.CreateAsync(userIdentity, user.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);

                return BadRequest(ModelState);
            }

            var roleResult = await _userManager.AddToRolesAsync(userIdentity, [nameof(Role.User)]);
            if (!roleResult.Succeeded) 
            {
                foreach(var error in roleResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                
                return BadRequest(ModelState);
            }
            
            return Accepted();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong in the {nameof(Register)}");
            return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="user">Login credentials.</param>
    /// <returns>
    /// <see cref="StatusCodes.Status202Accepted"/> if the user was authenticated successfully.
    /// <see cref="StatusCodes.Status400BadRequest"/> if the user was not authenticated successfully.
    /// <see cref="StatusCodes.Status401Unauthorized"/> if the user is not authorized to access the resource.
    /// <see cref="StatusCodes.Status500InternalServerError"/> if an error occurred during authentication.
    /// </returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginInfo user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var authenticatedUser = await _authManager.ValidateUser(user);
            
            if (authenticatedUser is null)
                return Unauthorized();

            var token = new { Token = await _authManager.CreateToken(authenticatedUser) };
            return Accepted(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {LoginName}: {ExMessage}", nameof(Login), ex.Message);
            return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
        }
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