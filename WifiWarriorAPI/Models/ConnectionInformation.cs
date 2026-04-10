namespace WifiWarriorAPI.Models;

/// <summary>
/// The model for connection information.
/// </summary>
public class ConnectionInformation : BaseEntity
{
    /// <summary>
    /// The connection information identifier.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The connection type identifier.
    /// </summary>
    public long ConnectionTypeId { get; set; }

    /// <summary>
    /// The wifi login details identifier.
    /// </summary>
    public long? WifiLoginDetailsId { get; set; }

    /// <summary>
    /// The connection type.
    /// </summary>
    public virtual ConnectionType? ConnectionType { get; set; }

    /// <summary>
    /// The wifi login details.
    /// </summary>
    public virtual WifiLoginDetails? WifiLoginDetails { get; set; }
}

/// <summary>
/// The type of connection.
/// </summary>
public enum ConnectionTypeEnum
{
    Open,
    Protected,
    Registration
}