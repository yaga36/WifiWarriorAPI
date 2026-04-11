using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.ConnectionTypes;

/// <summary>
/// Request payload for creating a connection type.
/// </summary>
public class CreateConnectionTypeRequest
{
    /// <summary>
    /// The connection type name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string Name { get; init; }
}
