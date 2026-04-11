using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.WifiDetails;

namespace WifiWarriorAPI.Services;

/// <summary>
/// Provides operations for managing Wi-Fi login detail records.
/// </summary>
public interface IWifiDetailsService
{
    /// <summary>
    /// Retrieves all Wi-Fi login details.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A read-only collection of <see cref="WifiDetailResponse"/> objects.
    /// </returns>
    Task<IReadOnlyCollection<WifiDetailResponse>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves Wi-Fi login details by identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the Wi-Fi details.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="WifiDetailResponse"/> when found; otherwise <see langword="null"/>.
    /// </returns>
    Task<WifiDetailResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates new Wi-Fi login details.
    /// </summary>
    /// <param name="request">The create Wi-Fi details request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the created <see cref="WifiDetailResponse"/> on success.
    /// </returns>
    Task<ServiceResult<WifiDetailResponse>> CreateAsync(CreateWifiDetailRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates existing Wi-Fi login details.
    /// </summary>
    /// <param name="id">The unique identifier of the Wi-Fi details.</param>
    /// <param name="request">The update Wi-Fi details request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating whether the update was successful.
    /// </returns>
    Task<ServiceResult<object>> UpdateAsync(long id, UpdateWifiDetailRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes Wi-Fi login details by identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the Wi-Fi details.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating whether the deletion was successful.
    /// </returns>
    Task<ServiceResult<object>> DeleteAsync(long id, CancellationToken cancellationToken);
}
