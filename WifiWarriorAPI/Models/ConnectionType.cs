using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models;

/// <summary>
/// The model for connection type.
/// </summary>
public class ConnectionType : BaseEntity
{
    /// <summary>
    /// The connection type identifier.
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// The connection type name.
    /// </summary>
    [MaxLength(50)]
    public required string Name { get; set; }

    /// <summary>
    /// The connection information rows that use this type.
    /// </summary>
    public virtual ICollection<ConnectionInformation> ConnectionInformations { get; set; } = [];
}