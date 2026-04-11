using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.ConnectionInformations;

/// <summary>
/// Request payload for updating connection information.
/// </summary>
public class UpdateConnectionInformationRequest
{
    [Required]
    public long ConnectionTypeId { get; init; }

    [Required]
    public long WifiLoginDetailsId { get; init; }
}