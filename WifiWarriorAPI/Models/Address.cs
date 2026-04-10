using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models;

/// <summary>
/// The model for the address.
/// </summary>
public class Address : BaseEntity
{
    /// <summary>
    /// The address identifier.
    /// </summary>
    public long Id { get; init; }
    
    /// <summary>
    /// The venue identifier.
    /// </summary>
    public long VenueId { get; set; }
    
    /// <summary>
    /// The address first line.
    /// </summary>
    [MaxLength(100)]
    public required string AddressLine1 { get; set; }
    
    /// <summary>
    /// The address second line.
    /// </summary>
    [MaxLength(100)]
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// The address town or city.
    /// </summary>
    [MaxLength(50)]
    public string? Area { get; set; }
    
    /// <summary>
    /// The address county.
    /// </summary>
    [MaxLength(50)]   
    public string? County { get; set; }
    
    /// <summary>
    /// The address postcode.
    /// </summary>
    [MaxLength(10)]
    public required string Postcode { get; set; }
    
    /// <summary>
    /// The address latitude.
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// The address longitude.
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// The connection information identifier.
    /// </summary>
    public long? ConnectionInformationId { get; set; }
    
    /// <summary>
    /// The venue.
    /// </summary>
    public virtual required Venue Venue { get; set; }
    
    /// <summary>
    /// The address connection information.
    /// </summary>
    public virtual ConnectionInformation? ConnectionInformation { get; set; }
}