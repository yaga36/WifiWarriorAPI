namespace WifiWarriorAPI.Models.Dtos.ConnectionInformations;

public static class ConnectionInformationMappings
{
    public static ConnectionInformationResponse ToResponse(this Models.ConnectionInformation model) =>
        new()
        {
            Id = model.Id,
            ConnectionTypeId = model.ConnectionTypeId,
            ConnectionTypeName = model.ConnectionType?.Name ?? string.Empty,
            WifiLoginDetailsId = model.WifiLoginDetailsId,
            WifiSsid = model.WifiLoginDetails?.Ssid ?? string.Empty
        };
}