using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("standings")]
public class Standing
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long TournamentId { get; set; }
    
    [ForeignKey("Player")]
    public long UserId { get; set; }
    
    public long GhostId { get; set; }

    public int Time { get; set; }

    [MaxLength(20)]
    public string HeroStyle { get; set; } = null!;

    [MaxLength(20)]
    public string WheelStyle { get; set; } = null!;

    [MaxLength(20)]
    public string EngineStyle { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Tournament Tournament { get; set; } = null!;
    public virtual Player Player { get; set; } = null!;
    public virtual Ghost Ghost { get; set; } = null!;
}