using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.ConnectionInformations;

namespace WifiWarriorAPI.Services;

/// <summary>
/// Provides operations for managing connection information records.
/// </summary>
public interface IConnectionInformationService
{
    /// <summary>
    /// Retrieves all connection information records.
    /// </summary>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of <see cref="ConnectionInformationResponse"/> objects.
    /// </returns>
    Task<IReadOnlyCollection<ConnectionInformationResponse>> GetAllAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves a connection information record by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection information.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ConnectionInformationResponse"/> if found; otherwise <c>null</c>.
    /// </returns>
    Task<ConnectionInformationResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new connection information record.
    /// </summary>
    /// <param name="request">
    /// The request containing the connection information details.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the created <see cref="ConnectionInformationResponse"/> on success.
    /// </returns>
    Task<ServiceResult<ConnectionInformationResponse>> CreateAsync(CreateConnectionInformationRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates an existing connection information record.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection information.
    /// </param>
    /// <param name="request">
    /// The request containing the updated connection information details.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating whether the update was successful.
    /// </returns>
    Task<ServiceResult<object>> UpdateAsync(long id, UpdateConnectionInformationRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes a connection information record by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the connection information.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating whether the deletion was successful.
    /// </returns>
    Task<ServiceResult<object>> DeleteAsync(long id, CancellationToken cancellationToken);
}