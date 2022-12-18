using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Services;

namespace WifiWarriorAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ApiDbContext _context;
    private readonly UserManager<Users> _userManager;
    private readonly IAuthManager _authManager;

    //TODO Add logging.
    // private readonly Logger<AccountController> _logger;

    public AccountController(ApiDbContext context, UserManager<Users> userManager, IAuthManager authManager)
    {
        _context = context;
        _userManager = userManager;
        _authManager = authManager;
        // _logger = logger;
    }

    // POST
    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserInfo user)
    {
        // _logger.LogInformation("Registration attempt for {UserEmail} ", user.Email);
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
                Password = user.Password
            };

            var result = await _userManager.CreateAsync(userIdentity, userIdentity.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            await _userManager.AddToRolesAsync(userIdentity, user.Roles);

            return Accepted();
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, $"Something went wrong in the {nameof(Register)}");
            return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
        }
    }

    // POST
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginInfo user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (!await _authManager.ValidateUser(user))
            {
                return Unauthorized(user);
            }

            var token = new { Token = await _authManager.CreateToken() };
            
            return Accepted(token);
        }
        catch (Exception ex)
        {
            return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
        }
    }
    
    [HttpGet]
    [Route("admin")]
    [Authorize(Roles = nameof(Role.Administrator))]
    public IActionResult GetAdmin()
    {
        return Ok("Admin Page");
    }
    
    [HttpGet]
    [Route("moderator")]
    [Authorize(Roles = nameof(Role.Moderator))]
    public IActionResult GetMod()
    {
        return Ok("Moderator Page");
    }
    
    [HttpGet]
    [Route("user")]
    [Authorize(Roles = nameof(Role.User))]
    public IActionResult GetUser()
    {
        return Ok("User Page");
    }
    
    [HttpGet]
    [AllowAnonymous]
    [Route("all")]
    public IActionResult GetPublic()
    {
        return Ok("Home Page");
    }
    
}