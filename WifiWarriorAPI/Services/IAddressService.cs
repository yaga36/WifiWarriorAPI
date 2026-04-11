using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Addresses;

namespace WifiWarriorAPI.Services;

/// <summary>
/// Provides operations for managing address records.
/// </summary>
public interface IAddressService
{
    /// <summary>
    /// Retrieves all addresses.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A read-only collection of <see cref="AddressResponse"/> items.
    /// </returns>
    Task<IReadOnlyCollection<AddressResponse>> GetAllAddressesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an address by identifier.
    /// </summary>
    /// <param name="id">The address identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// An <see cref="AddressResponse"/> when found; otherwise <see langword="null"/>.
    /// </returns>
    Task<AddressResponse?> GetAddressByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="request">The create address request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the created <see cref="AddressResponse"/> on success.
    /// On failure, includes an error message and status code.
    /// </returns>
    Task<ServiceResult<AddressResponse>> CreateAddressAsync(CreateAddressRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing address.
    /// </summary>
    /// <param name="id">The address identifier.</param>
    /// <param name="request">The update address request payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating outcome.
    /// Returns 204 on success, 404 when not found, or 400 for validation/business errors.
    /// </returns>
    Task<ServiceResult<object>> UpdateAddressAsync(long id, UpdateAddressRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an address by identifier.
    /// </summary>
    /// <param name="id">The address identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating outcome.
    /// Returns 204 on success, 404 when not found, or 400 for validation/business errors.
    /// </returns>
    Task<ServiceResult<object>> DeleteAddressAsync(long id, CancellationToken cancellationToken);
}