using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("tournaments")]
public class Tournament
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long TournamentGroupId { get; set; }

    [MaxLength(36)]
    public Guid Uuid { get; set; }

    public int EloGroup { get; set; }

    public int Cheaters { get; set; } = 0;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property for the relationship
    public virtual TournamentGroup TournamentGroup { get; set; } = null!;
}