using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Users;

namespace WifiWarriorAPI.Services;

public interface IUserService
{
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A read-only collection of <see cref="UserResponse"/> objects representing all users.
    /// </returns>
    Task<IReadOnlyCollection<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="UserResponse"/> when found; otherwise <see langword="null"/>.
    /// </returns>
    Task<UserResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="userRequest">The request containing user details.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the created <see cref="UserResponse"/> on success.
    /// On failure, includes an error message and status code (for example 400 or 409).
    /// </returns>
    Task<ServiceResult<UserResponse>> CreateUserAsync(CreateUserRequest userRequest, CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="userRequest">The request containing updated user values.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating outcome.
    /// Returns 204 on success or 404 when the user does not exist.
    /// </returns>
    Task<ServiceResult<object>> UpdateUserAsync(string id, UpdateUserRequest userRequest, CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes a user by identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating outcome.
    /// Returns 204 on success, 404 when missing, or 400 for validation/business rule failures.
    /// </returns>
    Task<ServiceResult<object>> DeleteUserAsync(string id, CancellationToken cancellationToken);
}