using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models;

/// <summary>
/// The model for venue.
/// </summary>
public class Venue : BaseEntity
{
    /// <summary>
    /// The venue identifier.
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// The venue name.
    /// </summary>
    [MaxLength(100)]
    public required string Name { get; set; }
}