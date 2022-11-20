namespace WifiWarriorAPI.Models;

public abstract class BaseEntity
{
    public long BaseEntityId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public long CreatedById { get; set; } = 1;
    public DateTime? UpdatedDate { get; set; }
    public long? UpdatedById { get; set; }
    public int? Status { get; set; } = 1;
}