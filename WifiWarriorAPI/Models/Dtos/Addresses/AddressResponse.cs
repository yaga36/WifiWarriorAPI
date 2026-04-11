namespace WifiWarriorAPI.Models.Dtos.Addresses;

/// <summary>
/// Represents address information associated with a venue.
/// </summary>
public class AddressResponse
{
    /// <summary>
    /// The unique identifier of the address.
    /// </summary>
    public long Id { get; init; }
    
    /// <summary>
    /// The first line of the address.
    /// </summary>
    public string AddressLine1 { get; init; } = string.Empty;
    
    /// <summary>
    /// The optional second line of the address.
    /// </summary>
    public string? AddressLine2 { get; init; }
    
    /// <summary>
    /// The local area or district.
    /// </summary>
    public string? Area { get; init; }
    
    /// <summary>
    /// The county or region.
    /// </summary>
    public string? County { get; init; }
    
    /// <summary>
    /// The postal code.
    /// </summary>
    public string Postcode { get; init; } = string.Empty;
    
    /// <summary>
    /// The latitude coordinate of the address.
    /// </summary>
    public double? Latitude { get; init; }
    
    /// <summary>
    /// The longitude coordinate of the address.
    /// </summary>
    public double? Longitude { get; init; }

    /// <summary>
    /// The identifier of the associated venue.
    /// </summary>
    public long VenueId { get; init; }
    
    /// <summary>
    /// The name of the associated venue.
    /// </summary>
    public string VenueName { get; init; } = string.Empty;

    /// <summary>
    /// The identifier of the associated connection information, if available.
    /// </summary>
    public long? ConnectionInformationId { get; init; }
    
    /// <summary>
    /// The name of the connection type, if available.
    /// </summary>
    public string? ConnectionTypeName { get; init; }
}