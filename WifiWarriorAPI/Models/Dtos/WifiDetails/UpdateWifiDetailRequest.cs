using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.WifiDetails;

/// <summary>
/// Request payload for updating Wi-Fi login details.
/// </summary>
public class UpdateWifiDetailRequest
{
    /// <summary>
    /// The Wi-Fi SSID.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Ssid { get; init; }

    /// <summary>
    /// The Wi-Fi password.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Password { get; init; }
}
