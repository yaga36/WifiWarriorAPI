namespace WifiWarriorAPI.Models.Dtos.Accounts;

/// <summary>
/// A response from the login operation.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// The login access token.
    /// </summary>
    public required string AccessToken { get; init; }
}