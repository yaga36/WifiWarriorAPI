namespace WifiWarriorAPI.Models;

public class ConnectionInformation : BaseEntity
{
    public long Id { get; set; }
    public long ConnectionTypeId { get; set; }
    public long? WifiLoginDetailsId { get; set; }
    public virtual ConnectionType? ConnectionType { get; set; }
    public virtual WifiLoginDetails? WifiLoginDetails { get; set; }
}

public enum ConnectionTypeEnum
{
    Open,
    Protected,
    Registration
}