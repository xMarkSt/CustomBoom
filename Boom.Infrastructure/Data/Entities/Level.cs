using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("level")]
public class Level
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [MaxLength(60)]
    public string DisplayName { get; set; } = null!;

    [MaxLength(100)]
    public string LevelId { get; set; } = null!;

    public long ThemeId { get; set; }

    public bool Online { get; set; } 

    public bool Custom { get; set; }

    [MaxLength(255)]
    public string? FilePath { get; set; }

    public short Version { get; set; } = 0;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("Background")]
    public long BgId { get; set; }

    // Navigation properties
    public virtual Theme Theme { get; set; } = null!;
    public virtual Theme Background { get; set; } = null!;
}