using User = WifiWarriorAPI.Models.Users;

namespace WifiWarriorAPI.Models.Dtos.Users;

public static class UserMappings
{
    /// <summary>
    /// Maps a <see cref="User"/> entity to a <see cref="UserResponse"/> DTO.
    /// </summary>
    /// <param name="user">
    /// The user entity to convert.
    /// </param>
    /// <returns>
    /// A <see cref="UserResponse"/> containing the mapped user data.
    /// </returns>
    public static UserResponse ToResponse(this User user) => 
        new ()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            PhoneNumber =  user.PhoneNumber ?? string.Empty,
        };
}