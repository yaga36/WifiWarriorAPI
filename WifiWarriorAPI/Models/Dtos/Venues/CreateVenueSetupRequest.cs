using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.Venues;

public class CreateVenueSetupRequest
{
    [Required]
    [MaxLength(100)]
    public required string VenueName { get; init; }

    [Required]
    public required CreateVenueSetupAddressRequest Address { get; init; }

    [Required]
    public required CreateVenueSetupConnectionRequest Connection { get; init; }
}

public class CreateVenueSetupAddressRequest
{
    [Required]
    [MaxLength(100)]
    public required string AddressLine1 { get; init; }

    [MaxLength(100)]
    public string? AddressLine2 { get; init; }

    [MaxLength(50)]
    public string? Area { get; init; }

    [MaxLength(50)]
    public string? County { get; init; }

    [Required]
    [MaxLength(10)]
    public required string Postcode { get; init; }

    public double? Latitude { get; init; }

    public double? Longitude { get; init; }
}

public class CreateVenueSetupConnectionRequest
{
    [Range(1, long.MaxValue)]
    public long ConnectionTypeId { get; init; }

    [MaxLength(100)]
    public string? Ssid { get; init; }

    [MaxLength(100)]
    public string? Password { get; init; }
}