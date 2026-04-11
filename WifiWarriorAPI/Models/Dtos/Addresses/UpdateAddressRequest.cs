using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.Addresses;

/// <summary>
/// Request payload for updating an address.
/// </summary>
public class UpdateAddressRequest
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

    [Required]
    public long VenueId { get; init; }

    public long? ConnectionInformationId { get; init; }
}