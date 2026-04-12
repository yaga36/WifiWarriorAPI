namespace WifiWarriorAPI.Models.Dtos.WifiDetails;

public static class WifiDetailMappings
{
    extension(WifiLoginDetails model)
    {
        public WifiDetailResponse ToResponse() =>
            new()
            {
                Id = model.Id,
                Ssid = model.Ssid,
                Password = model.EncryptedPassword
            };

        public WifiDetailResponse ToResponse(string decryptedPassword) =>
            new()
            {
                Id = model.Id,
                Ssid = model.Ssid,
                Password = decryptedPassword
            };
    }
}
