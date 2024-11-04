using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("articles")]
public class Article
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    [StringLength(1000)] public string? Message { get; set; }

    [StringLength(100)]
    public string? Link { get; set; }

    [StringLength(100)]
    public string? LinkTitle { get; set; }

    public bool Popup { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}