using WifiWarriorAPI.Models.Dtos.Addresses;
using WifiWarriorAPI.Models.Dtos.ConnectionInformations;
using WifiWarriorAPI.Models.Dtos.WifiDetails;

namespace WifiWarriorAPI.Models.Dtos.Venues;

public class VenueSetupResponse
{
    public required VenueResponse Venue { get; init; }

    public required AddressResponse Address { get; init; }

    public required ConnectionInformationResponse Connection { get; init; }

    public WifiDetailResponse? WifiDetails { get; init; }
}