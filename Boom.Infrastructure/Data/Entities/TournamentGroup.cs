using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("tournament_groups")]
public class TournamentGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [MaxLength(36)] public Guid Uuid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long LevelTargetId { get; set; }

    public bool NoSuper { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    // Navigation property
    public virtual LevelTarget LevelTarget { get; set; } = null!;
}