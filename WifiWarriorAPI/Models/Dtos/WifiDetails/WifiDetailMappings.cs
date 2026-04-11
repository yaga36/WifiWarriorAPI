namespace WifiWarriorAPI.Models.Dtos.WifiDetails;

public static class WifiDetailMappings
{
    public static WifiDetailResponse ToResponse(this WifiLoginDetails model) =>
        new()
        {
            Id = model.Id,
            Ssid = model.Ssid,
            Password = model.Password
        };
}
