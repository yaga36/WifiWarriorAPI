namespace WifiWarriorAPI.Models.Dtos;

/// <summary>
/// A standard error response for expected request failures.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// The human-readable error message.
    /// </summary>
    public string? Message { get; init; }
}
