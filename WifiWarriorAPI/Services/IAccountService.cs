using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Accounts;

namespace WifiWarriorAPI.Services;

/// <summary>
/// The account service, used for registering and logging in.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Creates a new user account using the provided registration details.
    /// </summary>
    /// <param name="registerRequest">
    /// The registration data, including user credentials and profile information.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the result of the operation,
    /// including success or failure information.
    /// </returns>
    Task<ServiceResult<object>> RegisterAsync(RegisterAccountRequest registerRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Authenticates a user and returns an authentication result.
    /// </summary>
    /// <param name="loginRequest">
    /// The request containing the user's login credentials.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the result of the login attempt,
    /// including success or failure information.
    /// </returns>
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginAccountRequest loginRequest,
        CancellationToken cancellationToken);
}