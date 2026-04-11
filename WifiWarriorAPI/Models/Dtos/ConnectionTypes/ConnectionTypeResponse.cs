namespace WifiWarriorAPI.Models.Dtos.ConnectionTypes;

/// <summary>
/// API response model for a connection type.
/// </summary>
public class ConnectionTypeResponse
{
    /// <summary>
    /// The unique identifier of the connection type.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// The connection type name.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}
