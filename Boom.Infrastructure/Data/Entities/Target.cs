using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("targets")]
public class Target : EntityBase
{
    [Column("type")]
    [StringLength(25)]
    public string Type { get; set; } = null!;
}