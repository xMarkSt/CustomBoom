using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("ghosts")]
public class Ghost : EntityBase
{
    [Column("data")]
    public byte[] Data { get; set; } = null!;
}