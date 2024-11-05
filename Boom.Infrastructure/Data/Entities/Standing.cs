using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("standings")]
public class Standing
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Column("tournament_id")]
    public long TournamentId { get; set; }
    
    [Column("user_id")]
    [ForeignKey("Player")]
    public long UserId { get; set; }
    
    [Column("ghost_id")]
    public long GhostId { get; set; }

    [Column("time")]
    public int Time { get; set; }

    [Column("hero_style")]
    [MaxLength(20)]
    public string HeroStyle { get; set; } = null!;

    [Column("wheel_style")]
    [MaxLength(20)]
    public string WheelStyle { get; set; } = null!;

    [Column("engine_style")]
    [MaxLength(20)]
    public string EngineStyle { get; set; } = null!;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Tournament Tournament { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;
    public virtual Ghost Ghost { get; set; } = null!;
}