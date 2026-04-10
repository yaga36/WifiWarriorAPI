namespace WifiWarriorAPI.Models;

/// <summary>
/// The base model for all entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// The base entity identifier.
    /// </summary>
    public long BaseEntityId { get; set; }

    /// <summary>
    /// The created date.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The user identifier that created the entity.
    /// </summary>
    public long CreatedById { get; set; } = 1;

    /// <summary>
    /// The updated date.
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// The user identifier that last updated the entity.
    /// </summary>
    public long? UpdatedById { get; set; }

    /// <summary>
    /// The entity status.
    /// </summary>
    public int? Status { get; set; } = 1;
}