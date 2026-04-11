using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.ConnectionInformations;

namespace WifiWarriorAPI.Services;

/// <inheritdoc />
public class ConnectionInformationService : IConnectionInformationService
{
    private readonly ApiDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionInformationService"/> class.
    /// </summary>
    /// <param name="context">
    /// The database context used to access and persist connection information data.
    /// </param>
    public ConnectionInformationService(ApiDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ConnectionInformationResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var rows = await _context.ConnectionInformation
            .AsNoTracking()
            .Include(x => x.ConnectionType)
            .Include(x => x.WifiLoginDetails)
            .ToListAsync(cancellationToken);

        return rows.Select(x => x.ToResponse()).ToList();
    }

    /// <inheritdoc />
    public async Task<ConnectionInformationResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var row = await _context.ConnectionInformation
            .AsNoTracking()
            .Include(x => x.ConnectionType)
            .Include(x => x.WifiLoginDetails)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return row?.ToResponse();
    }

    /// <inheritdoc />
    public async Task<ServiceResult<ConnectionInformationResponse>> CreateAsync(CreateConnectionInformationRequest request, CancellationToken cancellationToken)
    {
        var entity = new ConnectionInformation
        {
            ConnectionTypeId = request.ConnectionTypeId,
            WifiLoginDetailsId = request.WifiLoginDetailsId,
            CreatedDate = DateTime.UtcNow
        };

        _context.ConnectionInformation.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<ConnectionInformationResponse>(true, entity.ToResponse(), StatusCode: StatusCodes.Status201Created);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> UpdateAsync(long id, UpdateConnectionInformationRequest request, CancellationToken cancellationToken)
    {
        var entity = await _context.ConnectionInformation.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return new ServiceResult<object>(false, Error: "Connection information not found", StatusCode: StatusCodes.Status404NotFound);

        entity.ConnectionTypeId = request.ConnectionTypeId;
        entity.WifiLoginDetailsId = request.WifiLoginDetailsId;
        entity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _context.ConnectionInformation.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return new ServiceResult<object>(false, Error: "Connection information not found", StatusCode: StatusCodes.Status404NotFound);

        _context.ConnectionInformation.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
    }
}