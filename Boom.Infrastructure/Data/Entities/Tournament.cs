using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("tournaments")]
public class Tournament
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Column("tournament_group_id")]
    public long TournamentGroupId { get; set; }

    [Column("uuid")]
    [MaxLength(36)]
    public Guid Uuid { get; set; }

    [Column("elo_group")]
    public int EloGroup { get; set; }

    [Column("cheaters")]
    public int Cheaters { get; set; } = 0;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation property for the relationship
    public virtual TournamentGroup TournamentGroup { get; set; } = null!;
}