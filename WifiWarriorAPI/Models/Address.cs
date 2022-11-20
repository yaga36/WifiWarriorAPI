namespace WifiWarriorAPI.Models;

public class Address : BaseEntity
{
    public long Id { get; set; }
    public long VenueId { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Area { get; set; }
    public string? County { get; set; }
    public string Postcode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public long? ConnectionInformationId { get; set; }
    public virtual Venue Venue { get; set; }
    public virtual ConnectionInformation? ConnectionInformation { get; set; }
}