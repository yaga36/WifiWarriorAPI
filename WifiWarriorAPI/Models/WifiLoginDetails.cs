using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models;

/// <summary>
/// The model for wifi login details.
/// </summary>
public class WifiLoginDetails : BaseEntity
{
    /// <summary>
    /// The wifi login details identifier.
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// The wifi login SSID.
    /// </summary>
    [MaxLength(100)]
    public required string Ssid { get; set; }
    
    /// <summary>
    /// The encrypted wifi login password.
    /// </summary>
    [MaxLength(100)]
    public required string EncryptedPassword { get; set; }
}
