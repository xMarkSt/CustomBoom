using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("tournament_groups")]
public class TournamentGroup : EntityBase
{
    [Column("uuid")]
    [MaxLength(36)] public Guid Uuid { get; set; }

    [Column("level_target_id")]
    public long LevelTargetId { get; set; }

    [Column("no_super")]
    public bool NoSuper { get; set; } = false;

    /// <summary>
    /// Start date / time in UTC
    /// </summary>
    [Column("starts_at")]
    public DateTime StartsAt { get; set; }

    /// <summary>
    /// Ends at date/time in UTC
    /// </summary>
    [Column("ends_at")]
    public DateTime EndsAt { get; set; }

    // Navigation property
    public virtual LevelTarget LevelTarget { get; set; } = null!;
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}