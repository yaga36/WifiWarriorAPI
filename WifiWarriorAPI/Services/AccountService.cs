using Microsoft.AspNetCore.Identity;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Accounts;

namespace WifiWarriorAPI.Services;

/// <inheritdoc/>
public class AccountService : IAccountService
{
    private readonly UserManager<Users> _userManager;
    private readonly IAuthManager _authManager;
    private readonly ILogger<AccountService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountService"/> class.
    /// </summary>
    /// <param name="userManager">Manages user-related operations.</param>
    /// <param name="authManager">Handles authentication and token generation.</param>
    /// <param name="logger">Provides logging for the service.</param>
    public AccountService(UserManager<Users> userManager, IAuthManager authManager, ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _authManager = authManager;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<object>> RegisterAsync(RegisterAccountRequest registerRequest, CancellationToken cancellationToken)
    {
        try
        {
            var user = new Users
            {
                UserName = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                NormalizedEmail = registerRequest.Email.ToUpperInvariant(),
                PhoneNumber = registerRequest.PhoneNumber
            };

            var createResult = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!createResult.Succeeded)
                return new ServiceResult<object>(false, Error: string.Join("; ", createResult.Errors.Select(error => error.Description)), StatusCode: StatusCodes.Status400BadRequest);

            var roleResult = await _userManager.AddToRolesAsync(user, [nameof(Role.User)]);
            if (!roleResult.Succeeded) 
                return new ServiceResult<object>(false, Error: string.Join("; ", roleResult.Errors.Select(e => e.Description)), StatusCode: StatusCodes.Status400BadRequest);
            
            return new ServiceResult<object>(true, Value: null, StatusCode: StatusCodes.Status202Accepted);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed");
            return new ServiceResult<object>(false, "Internal server error", StatusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginAccountRequest loginRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var loginInfo = new LoginInfo
            {
                Email = loginRequest.Email,
                Password = loginRequest.Password
            };
            
            var authenticatedUser = await _authManager.ValidateUser(loginInfo);
            
            if (authenticatedUser is null)
                return new ServiceResult<LoginResponse>(false, Error: "Invalid email or password.", StatusCode: StatusCodes.Status401Unauthorized);

            var accessToken = await _authManager.CreateToken(authenticatedUser);
            return new ServiceResult<LoginResponse>(true, new LoginResponse{ AccessToken = accessToken }, StatusCode: StatusCodes.Status202Accepted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,  $"Login failed");
            return new ServiceResult<LoginResponse>(false, Error: "Internal server error", StatusCode: StatusCodes.Status500InternalServerError);
        }
    }
}