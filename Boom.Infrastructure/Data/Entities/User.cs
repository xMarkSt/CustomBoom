using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [StringLength(191)]
    public string Name { get; set; } = null!;
    
    [StringLength(191)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public DateTime? EmailVerifiedAt { get; set; }

    [StringLength(191)]
    public string Password { get; set; } = null!;

    [StringLength(100)]
    public string? RememberToken { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}