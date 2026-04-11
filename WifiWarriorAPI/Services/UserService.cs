using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Users;

namespace WifiWarriorAPI.Services;

public class UserService : IUserService
{
    private readonly ApiDbContext _context;
    private readonly UserManager<Users> _userManager;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="logger">The user service logger.</param>
    public UserService(ApiDbContext context, UserManager<Users> userManager, ILogger<UserService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);
        return users.Select(user => user.ToResponse()).ToList();
    }
    
    /// <inheritdoc />
    public async Task<UserResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return user?.ToResponse();
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<UserResponse>> CreateUserAsync(CreateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = new Users
        {
            FirstName = userRequest.FirstName,
            LastName = userRequest.LastName,
            Email = userRequest.Email,
            PhoneNumber = userRequest.PhoneNumber,
            UserName = userRequest.Email,
            NormalizedEmail = userRequest.Email.ToUpperInvariant(),
            NormalizedUserName = userRequest.Email.ToUpperInvariant(),
        };
        
        var createResult = await _userManager.CreateAsync(user, userRequest.Password);
        if (createResult.Succeeded)
            return new ServiceResult<UserResponse>(true, user.ToResponse(), StatusCode: StatusCodes.Status201Created);
        
        var errors = createResult.Errors.ToList();
        var errorText = string.Join(", ", errors.Select(e => e.Description));
            
        var hasDuplicate = errors.Any(e =>
            e.Code is "DuplicateUserName" or "DuplicateEmail");

        var statusCode = hasDuplicate
            ? StatusCodes.Status409Conflict
            : StatusCodes.Status400BadRequest;

        _logger.LogWarning("Create user failed ({StatusCode}): {Error}", statusCode, errorText);

        return new ServiceResult<UserResponse>(
            false,
            Error: errorText,
            StatusCode: statusCode);

    }

    /// <inheritdoc />
    public async Task<ServiceResult<object>> UpdateUserAsync(string id, UpdateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existingUser is null)
            return new ServiceResult<object>(false, Error: "User not found", StatusCode: StatusCodes.Status404NotFound);
        
        existingUser.FirstName = userRequest.FirstName;
        existingUser.LastName = userRequest.LastName;
        existingUser.Email = userRequest.Email;
        existingUser.PhoneNumber = userRequest.PhoneNumber;
        existingUser.UserName = userRequest.Email;
        existingUser.NormalizedEmail = userRequest.Email.ToUpperInvariant();
        existingUser.NormalizedUserName = userRequest.Email.ToUpperInvariant();

        var updateResult = await _userManager.UpdateAsync(existingUser);
        if (updateResult.Succeeded) 
            return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
        
        var error = string.Join(", ", updateResult.Errors.Select(e => e.Description));
        _logger.LogWarning("Update user failed: {Error}", error);
        return new ServiceResult<object>(false, Error: error, StatusCode: StatusCodes.Status400BadRequest);

    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<object>> DeleteUserAsync(string id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
            return new ServiceResult<object>(false, Error: "User not found", StatusCode: StatusCodes.Status404NotFound);

        var deleteResult = await _userManager.DeleteAsync(user);
        if (deleteResult.Succeeded) 
            return new ServiceResult<object>(true, StatusCode: StatusCodes.Status204NoContent);
        
        var error = string.Join(", ", deleteResult.Errors.Select(x => x.Description));
        _logger.LogWarning("Delete user failed: {Error}", error);
        return new ServiceResult<object>(false, Error: error, StatusCode: StatusCodes.Status400BadRequest);

    }
}