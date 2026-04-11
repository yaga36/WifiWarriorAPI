using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.ConnectionTypes;

namespace WifiWarriorAPI.Services;

/// <summary>
/// Provides operations for managing connection type records.
/// </summary>
public interface IConnectionTypeService
{
    /// <summary>
    /// Retrieves all connection types.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A read-only collection of <see cref="ConnectionTypeResponse"/> objects.
    /// </returns>
    Task<IReadOnlyCollection<ConnectionTypeResponse>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a connection type by identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the connection type.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ConnectionTypeResponse"/> when found; otherwise <see langword="null"/>.
    /// </returns>
    Task<ConnectionTypeResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new connection type.
    /// </summary>
    /// <param name="request">The create connection type request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the created <see cref="ConnectionTypeResponse"/> on success.
    /// </returns>
    Task<ServiceResult<ConnectionTypeResponse>> CreateAsync(CreateConnectionTypeRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing connection type.
    /// </summary>
    /// <param name="id">The unique identifier of the connection type.</param>
    /// <param name="request">The update connection type request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating whether the update was successful.
    /// </returns>
    Task<ServiceResult<object>> UpdateAsync(long id, UpdateConnectionTypeRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a connection type by identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the connection type.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating whether the deletion was successful.
    /// </returns>
    Task<ServiceResult<object>> DeleteAsync(long id, CancellationToken cancellationToken);
}
