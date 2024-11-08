using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("articles")]
public class Article : EntityBase
{
    [Column("title")]
    [StringLength(100)]
    public string? Title { get; set; }

    [Column("message")]
    [StringLength(1000)]
    public string? Message { get; set; }

    [Column("link")]
    [StringLength(100)]
    public string? Link { get; set; }

    [Column("link_title")]
    [StringLength(100)]
    public string? LinkTitle { get; set; }

    [Column("popup")]
    public bool Popup { get; set; }
}