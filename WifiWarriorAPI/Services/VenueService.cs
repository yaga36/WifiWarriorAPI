using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Addresses;
using WifiWarriorAPI.Models.Dtos.ConnectionInformations;
using WifiWarriorAPI.Models.Dtos.Venues;
using WifiWarriorAPI.Models.Dtos.WifiDetails;

namespace WifiWarriorAPI.Services;

/// <inheritdoc />
public class VenueService : IVenueService
{
    private readonly ApiDbContext _context;
    private readonly ICredentialsProtector _protector;

    /// <summary>
    /// Initializes a new instance of the <see cref="VenueService"/> class.
    /// </summary>
    /// <param name="context">
    /// The database context.
    /// </param>
    public VenueService(ApiDbContext context, ICredentialsProtector protector)
    {
        _context = context;
        _protector = protector;
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
    public async Task<IReadOnlyCollection<VenueSetupResponse>> GetAllVenueSetupsAsync(CancellationToken cancellationToken)
    {
        var addresses = await _context.Addresses
            .AsNoTracking()
            .Include(x => x.Venue)
            .Include(x => x.ConnectionInformation!)
                .ThenInclude(ci => ci.ConnectionType)
            .Include(x => x.ConnectionInformation!)
                .ThenInclude(ci => ci.WifiLoginDetails)
            .Where(x => x.Venue != null && x.ConnectionInformation != null)
            .ToListAsync(cancellationToken);

        return addresses
            .Select(MapVenueSetupResponse)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<VenueSetupResponse?> GetVenueSetupByIdAsync(long id, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses
            .AsNoTracking()
            .Include(x => x.Venue)
            .Include(x => x.ConnectionInformation!)
                .ThenInclude(ci => ci.ConnectionType)
            .Include(x => x.ConnectionInformation!)
                .ThenInclude(ci => ci.WifiLoginDetails)
            .FirstOrDefaultAsync(x => x.VenueId == id, cancellationToken);

        if (address?.Venue is null || address.ConnectionInformation is null)
            return null;

        return MapVenueSetupResponse(address);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<VenueSetupResponse>> CreateVenueSetupAsync(CreateVenueSetupRequest request, CancellationToken cancellationToken)
    {
        var connectionType = await _context.ConnectionTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Connection.ConnectionTypeId, cancellationToken);

        if (connectionType is null)
            return new ServiceResult<VenueSetupResponse>(false, Error: "Connection type not found", StatusCode: StatusCodes.Status400BadRequest);

        var requiresCredentials = !string.Equals(connectionType.Name, "Open", StringComparison.OrdinalIgnoreCase);
        if (requiresCredentials && (string.IsNullOrWhiteSpace(request.Connection.Ssid) || string.IsNullOrWhiteSpace(request.Connection.Password)))
            return new ServiceResult<VenueSetupResponse>(false, Error: "SSID and password are required for protected or login connections", StatusCode: StatusCodes.Status400BadRequest);

        await using var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        var now = DateTime.UtcNow;

        var venue = new Venue
        {
            Name = request.VenueName,
            CreatedDate = now
        };
        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);

        WifiLoginDetails? wifiDetails = null;
        if (requiresCredentials)
        {
            wifiDetails = new WifiLoginDetails
            {
                Ssid = request.Connection.Ssid!,
                EncryptedPassword = _protector.Encrypt(request.Connection.Password!),
                CreatedDate = now
            };

            _context.WifiLoginDetails.Add(wifiDetails);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var connectionInformation = new ConnectionInformation
        {
            ConnectionTypeId = connectionType.Id,
            WifiLoginDetailsId = wifiDetails?.Id,
            CreatedDate = now
        };
        _context.ConnectionInformation.Add(connectionInformation);
        await _context.SaveChangesAsync(cancellationToken);

        var address = new Address
        {
            AddressLine1 = request.Address.AddressLine1,
            AddressLine2 = request.Address.AddressLine2,
            Area = request.Address.Area,
            County = request.Address.County,
            Postcode = request.Address.Postcode,
            Latitude = request.Address.Latitude,
            Longitude = request.Address.Longitude,
            VenueId = venue.Id,
            ConnectionInformationId = connectionInformation.Id,
            CreatedDate = now
        };
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        if (transaction is not null)
            await transaction.CommitAsync(cancellationToken);

        connectionInformation.ConnectionType = connectionType;
        connectionInformation.WifiLoginDetails = wifiDetails;
        address.Venue = venue;
        address.ConnectionInformation = connectionInformation;

        return new ServiceResult<VenueSetupResponse>(
            true,
            new VenueSetupResponse
            {
                Venue = venue.ToResponse(),
                Address = address.ToResponse(),
                Connection = connectionInformation.ToResponse(),
                WifiDetails = wifiDetails?.ToResponse(request.Connection.Password!)
            },
            StatusCode: StatusCodes.Status201Created);
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

    private VenueSetupResponse MapVenueSetupResponse(Address address)
    {
        ArgumentNullException.ThrowIfNull(address.Venue);
        ArgumentNullException.ThrowIfNull(address.ConnectionInformation);

        var wifiDetails = address.ConnectionInformation.WifiLoginDetails;

        return new VenueSetupResponse
        {
            Venue = address.Venue.ToResponse(),
            Address = address.ToResponse(),
            Connection = address.ConnectionInformation.ToResponse(),
            WifiDetails = wifiDetails?.ToResponse(_protector.Decrypt(wifiDetails.EncryptedPassword))
        };
    }
}