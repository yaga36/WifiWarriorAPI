using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.ConnectionTypes;

/// <summary>
/// Request payload for updating a connection type.
/// </summary>
public class UpdateConnectionTypeRequest
{
    /// <summary>
    /// The connection type name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string Name { get; init; }
}
