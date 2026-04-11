using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.Venues;

public class CreateVenueRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; init; } = string.Empty;
}