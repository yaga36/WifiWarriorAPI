using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.ConnectionInformations;

/// <summary>
/// Request payload for creating connection information.
/// </summary>
public class CreateConnectionInformationRequest
{
    [Required]
    public long ConnectionTypeId { get; init; }

    [Required]
    public long WifiLoginDetailsId { get; init; }
}