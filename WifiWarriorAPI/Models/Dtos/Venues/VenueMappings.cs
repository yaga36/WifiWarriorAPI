namespace WifiWarriorAPI.Models.Dtos.Venues;

public static class VenueMappings
{
    public static VenueResponse ToResponse(this Venue venue) => 
        new () { Id = venue.Id, Name = venue.Name };
}