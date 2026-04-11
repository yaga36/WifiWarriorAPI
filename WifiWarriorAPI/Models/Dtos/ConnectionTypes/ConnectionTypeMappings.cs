namespace WifiWarriorAPI.Models.Dtos.ConnectionTypes;

public static class ConnectionTypeMappings
{
    public static ConnectionTypeResponse ToResponse(this ConnectionType model) =>
        new()
        {
            Id = model.Id,
            Name = model.Name
        };
}
