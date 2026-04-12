using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.WifiDetails;

namespace WifiWarriorAPI.Services;

/// <inheritdoc />
public class WifiDetailsService : IWifiDetailsService
{
    private readonly ApiDbContext _context;
    private readonly ICredentialsProtector _protector;

    /// <summary>
    /// Initializes a new instance of the <see cref="WifiDetailsService"/> class.
    /// </summary>
    /// <param name="context">
    /// The database context used to access and persist Wi-Fi detail data.
    /// </param>
    /// <param name="protector">
    /// The credentials protector for encrypting/decrypting passwords.
    /// </param>
    public WifiDetailsService(ApiDbContext context, ICredentialsProtector protector)
    {
        _context = context;
        _protector = protector;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<WifiDetailResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var wifiDetails = await _context.WifiLoginDetails
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return wifiDetails.Select(wd => wd.ToResponse(_protector.Decrypt(wd.EncryptedPassword))).ToList();
    }

    /// <inheritdoc />
    public async Task<WifiDetailResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var wifiDetail = await _context.WifiLoginDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(wd => wd.Id == id, cancellationToken);
        
        return wifiDetail?.ToResponse(_protector.Decrypt(wifiDetail.EncryptedPassword));
    }

    /// <inheritdoc />
    public async Task<ServiceResult<WifiDetailResponse>> CreateAsync(CreateWifiDetailRequest request, CancellationToken cancellationToken)
    {
        var entity = new WifiLoginDetails
        {
            Ssid = request.Ssid,
            EncryptedPassword = _protector.Encrypt(request.Password),
            CreatedDate = DateTime.UtcNow
        };

        _context.WifiLoginDetails.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<WifiDetailResponse>(true, entity.ToResponse(request.Password), StatusCode: StatusCodes.Status201Created);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> UpdateAsync(long id, UpdateWifiDetailRequest request, CancellationToken cancellationToken)
    {
        var entity = await _context.WifiLoginDetails.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return new ServiceResult<object>(false, Error: "Wi-Fi details not found", StatusCode: StatusCodes.Status404NotFound);

        entity.Ssid = request.Ssid;
        entity.EncryptedPassword = _protector.Encrypt(request.Password);
        entity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _context.WifiLoginDetails.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return new ServiceResult<object>(false, Error: "Wi-Fi details not found", StatusCode: StatusCodes.Status404NotFound);

        _context.WifiLoginDetails.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
    }
}
