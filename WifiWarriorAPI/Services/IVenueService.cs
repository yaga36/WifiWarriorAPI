using WifiWarriorAPI.Models.Dtos.Venues;

namespace WifiWarriorAPI.Services;

/// <summary>
/// Provides operations for managing venue records.
/// </summary>
public interface IVenueService
{
    /// <summary>
    /// Retrieves all venues.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection of venues; empty when no venues exist.</returns>
    Task<IReadOnlyCollection<VenueResponse>> GetAllVenuesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets venue by identifier.
    /// </summary>
    /// <param name="id">The venue identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The venue when found; otherwise <see langword="null"/>.</returns>
    Task<VenueResponse?> GetVenueByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new venue.
    /// </summary>
    /// <param name="venueRequest">The venue to create request data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created venue, including generated values.</returns>
    Task<VenueResponse> CreateVenueAsync(CreateVenueRequest venueRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing venue.
    /// </summary>
    /// <param name="id">The target venue identifier.</param>
    /// <param name="venueRequest">The updated venue request data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> when the update succeeds;
    /// otherwise <see langword="false"/> when the id is invalid or the venue does not exist.
    /// </returns>
    Task<bool> UpdateVenueAsync(long id, UpdateVenueRequest venueRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a venue by identifier.
    /// </summary>
    /// <param name="id">The venue identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> when the venue is deleted;
    /// otherwise <see langword="false"/> when no matching venue exists.
    /// </returns>
    Task<bool> DeleteVenueAsync(long id, CancellationToken cancellationToken);
}