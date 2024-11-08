using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("themes")]
public class Theme : EntityBase
{
    [Column("name")]
    [StringLength(30)]
    public string Name { get; set; } = null!;

    [Column("bg_name")]
    [StringLength(50)]
    public string? BgName { get; set; }
}