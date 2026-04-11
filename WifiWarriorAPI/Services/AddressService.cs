using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Addresses;

namespace WifiWarriorAPI.Services;

/// <inheritdoc/>
public class AddressService : IAddressService
{
    private readonly ApiDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressService"/> class.
    /// </summary>
    /// <param name="context">
    /// The database context used to access and persist address data.
    /// </param>
    public AddressService(ApiDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<AddressResponse>> GetAllAddressesAsync(CancellationToken cancellationToken)
    {
        var addresses = await _context.Addresses
            .AsNoTracking()
            .Include(x => x.Venue)
            .Include(x => x.ConnectionInformation!)
                .ThenInclude(ci => ci.ConnectionType)
            .Include(x => x.ConnectionInformation!)
                .ThenInclude(ci => ci.WifiLoginDetails)
            .ToListAsync(cancellationToken);

        return addresses.Select(x => x.ToResponse()).ToList();
    }
    
    /// <inheritdoc/>
    public async Task<AddressResponse?> GetAddressByIdAsync(long id, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses
            .AsNoTracking()
            .Include(x => x.Venue)
            .Include(x => x.ConnectionInformation!)
                .ThenInclude(ci => ci.ConnectionType)
            .Include(x => x.ConnectionInformation!)
                .ThenInclude(ci => ci.WifiLoginDetails)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return address?.ToResponse();
    }
    
    /// <inheritdoc/>
    public async Task<ServiceResult<AddressResponse>> CreateAddressAsync(CreateAddressRequest request, CancellationToken cancellationToken)
    {
        var address = new Address
        {
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            Area = request.Area,
            County = request.County,
            Postcode = request.Postcode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            VenueId = request.VenueId,
            ConnectionInformationId = request.ConnectionInformationId,
            CreatedDate = DateTime.UtcNow
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<AddressResponse>(true, address.ToResponse(), StatusCode: StatusCodes.Status201Created);
    }
    
    /// <inheritdoc/>
    public async Task<ServiceResult<object>> UpdateAddressAsync(long id, UpdateAddressRequest request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (address is null)
            return new ServiceResult<object>(false, Error: "Address not found", StatusCode: StatusCodes.Status404NotFound);

        address.AddressLine1 = request.AddressLine1;
        address.AddressLine2 = request.AddressLine2;
        address.Area = request.Area;
        address.County = request.County;
        address.Postcode = request.Postcode;
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        address.VenueId = request.VenueId;
        address.ConnectionInformationId = request.ConnectionInformationId;
        address.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
    }
    
    /// <inheritdoc/>
    public async Task<ServiceResult<object>> DeleteAddressAsync(long id, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (address is null)
            return new ServiceResult<object>(false, Error: "Address not found", StatusCode: StatusCodes.Status404NotFound);

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
    }
}