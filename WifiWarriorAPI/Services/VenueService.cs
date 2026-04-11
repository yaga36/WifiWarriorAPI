using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Venues;

namespace WifiWarriorAPI.Services;

/// <inheritdoc />
public class VenueService : IVenueService
{
    private readonly ApiDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="VenueService"/> class.
    /// </summary>
    /// <param name="context">
    /// The database context.
    /// </param>
    public VenueService(ApiDbContext context)
    {
        _context = context;
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<VenueResponse>> GetAllVenuesAsync(CancellationToken cancellationToken)
    {
        var venues = await _context.Venues.AsNoTracking().ToListAsync(cancellationToken);
        return venues.Select(venue => venue.ToResponse()).ToList();
    }

    /// <inheritdoc />
    public async Task<VenueResponse?> GetVenueByIdAsync(long id, CancellationToken cancellationToken)
    {
        var venue = await _context.Venues.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return venue?.ToResponse();
    }

    /// <inheritdoc />
    public async Task<VenueResponse> CreateVenueAsync(CreateVenueRequest venueRequest, CancellationToken cancellationToken)
    {
        var venue = new Venue
        {
            Name = venueRequest.Name,
            CreatedDate = DateTime.UtcNow
        };
        
        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);
        return venue.ToResponse();
    }

    /// <inheritdoc />
    public async Task<bool> UpdateVenueAsync(long id, UpdateVenueRequest venueRequest, CancellationToken cancellationToken)
    {
        var existingVenue = await _context.Venues.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existingVenue is null)
            return false;
        
        existingVenue.Name = venueRequest.Name;
        existingVenue.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    /// <inheritdoc />
    public async Task<bool> DeleteVenueAsync(long id, CancellationToken cancellationToken)
    {
        var venue = await _context.Venues.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (venue is null)
            return false;
        
        _context.Venues.Remove(venue);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}