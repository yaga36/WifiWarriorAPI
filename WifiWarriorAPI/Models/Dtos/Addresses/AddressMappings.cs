namespace WifiWarriorAPI.Models.Dtos.Addresses;

public static class AddressMappings
{
    public static AddressResponse ToResponse(this Address address) =>
        new()
        {
            Id = address.Id,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            Area = address.Area,
            County = address.County,
            Postcode = address.Postcode,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            VenueId = address.VenueId,
            VenueName = address.Venue?.Name ?? string.Empty,
            ConnectionInformationId = address.ConnectionInformationId,
            ConnectionTypeName = address.ConnectionInformation?.ConnectionType?.Name
        };
}