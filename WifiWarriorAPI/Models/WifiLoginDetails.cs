namespace WifiWarriorAPI.Models;

public class WifiLoginDetails : BaseEntity
{
    public long Id { get; set; }
    public string Ssid { get; set; }
    public string Password { get; set; }
}