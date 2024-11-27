using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("levels")]
public class Level : EntityBase
{
    [Column("display_name")]
    [MaxLength(60)]
    public string DisplayName { get; set; } = null!;

    [Column("level_id")]
    [MaxLength(100)]
    public string LevelId { get; set; } = null!;

    [Column("theme_id")]
    public long ThemeId { get; set; }

    [Column("online")]
    public bool Online { get; set; }

    [Column("custom")]
    public bool Custom { get; set; }

    [Column("file_path")]
    [MaxLength(255)]
    public string? FilePath { get; set; }

    [Column("version")]
    public short Version { get; set; } = 0;

    [Column("bg_id")]
    [ForeignKey("Background")]
    public long BgId { get; set; }

    // Navigation properties
    public virtual Theme Theme { get; set; } = null!;
    public virtual Theme Background { get; set; } = null!;
}