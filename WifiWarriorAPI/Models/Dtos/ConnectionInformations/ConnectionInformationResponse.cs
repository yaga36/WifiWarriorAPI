namespace WifiWarriorAPI.Models.Dtos.ConnectionInformations;

/// <summary>
/// API response model for connection information.
/// </summary>
public class ConnectionInformationResponse
{
    public long Id { get; init; }
    public long ConnectionTypeId { get; init; }
    public string ConnectionTypeName { get; init; } = string.Empty;
    public long? WifiLoginDetailsId { get; init; }
    public string? WifiSsid { get; init; } = string.Empty;
}