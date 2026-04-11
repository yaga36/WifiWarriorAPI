namespace WifiWarriorAPI.Models.Dtos.Venues;

public static class VenueMappings
{
    /// <summary>
    /// Maps a <see cref="Venue"/> entity to a <see cref="VenueResponse"/> DTO.
    /// </summary>
    /// <param name="venue">
    /// The venue entity to convert.
    /// </param>
    /// <returns>
    /// A <see cref="VenueResponse"/> representing the mapped venue data.
    /// </returns>
    public static VenueResponse ToResponse(this Venue venue) => 
        new () { Id = venue.Id, Name = venue.Name };
}