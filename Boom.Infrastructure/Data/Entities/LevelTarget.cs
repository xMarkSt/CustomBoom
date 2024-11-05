using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("level_target")]
public class LevelTarget
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Column("level_id")]
    public long LevelId { get; set; }

    [Column("target_id")]
    public long TargetId { get; set; }

    [Column("target_amount")]
    public int? TargetAmount { get; set; }

    [Column("order")]
    public int Order { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Level Level { get; set; } = null!;
    public virtual Target Target { get; set; } = null!;
}