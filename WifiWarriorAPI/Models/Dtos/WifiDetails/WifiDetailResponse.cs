namespace WifiWarriorAPI.Models.Dtos.WifiDetails;

/// <summary>
/// API response model for Wi-Fi login details.
/// </summary>
public class WifiDetailResponse
{
    /// <summary>
    /// The unique identifier of the Wi-Fi details.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// The Wi-Fi SSID.
    /// </summary>
    public string Ssid { get; init; } = string.Empty;

    /// <summary>
    /// The Wi-Fi password.
    /// </summary>
    public string Password { get; init; } = string.Empty;
}
