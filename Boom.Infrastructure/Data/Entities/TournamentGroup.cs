using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("tournament_groups")]
public class TournamentGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Column("uuid")]
    [MaxLength(36)] public Guid Uuid { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("level_target_id")]
    public long LevelTargetId { get; set; }

    [Column("no_super")]
    public bool NoSuper { get; set; }

    [Column("starts_at")]
    public DateTime StartsAt { get; set; }

    [Column("ends_at")]
    public DateTime EndsAt { get; set; }

    // Navigation property
    public virtual LevelTarget LevelTarget { get; set; } = null!;
}