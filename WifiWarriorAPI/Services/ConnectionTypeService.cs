using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.ConnectionTypes;

namespace WifiWarriorAPI.Services;

/// <inheritdoc />
public class ConnectionTypeService : IConnectionTypeService
{
    private readonly ApiDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionTypeService"/> class.
    /// </summary>
    /// <param name="context">
    /// The database context used to access and persist connection type data.
    /// </param>
    public ConnectionTypeService(ApiDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ConnectionTypeResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var rows = await _context.ConnectionTypes
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows.Select(x => x.ToResponse()).ToList();
    }

    /// <inheritdoc />
    public async Task<ConnectionTypeResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var row = await _context.ConnectionTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return row?.ToResponse();
    }

    /// <inheritdoc />
    public async Task<ServiceResult<ConnectionTypeResponse>> CreateAsync(CreateConnectionTypeRequest request, CancellationToken cancellationToken)
    {
        var entity = new ConnectionType
        {
            Name = request.Name,
            CreatedDate = DateTime.UtcNow
        };

        _context.ConnectionTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<ConnectionTypeResponse>(true, entity.ToResponse(), StatusCode: StatusCodes.Status201Created);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> UpdateAsync(long id, UpdateConnectionTypeRequest request, CancellationToken cancellationToken)
    {
        var entity = await _context.ConnectionTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return new ServiceResult<object>(false, Error: "Connection type not found", StatusCode: StatusCodes.Status404NotFound);

        entity.Name = request.Name;
        entity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _context.ConnectionTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return new ServiceResult<object>(false, Error: "Connection type not found", StatusCode: StatusCodes.Status404NotFound);

        _context.ConnectionTypes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
    }
}
