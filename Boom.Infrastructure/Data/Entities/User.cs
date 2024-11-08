using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("users")]
public class User : EntityBase
{
    [Column("name")]
    [StringLength(191)]
    public string Name { get; set; } = null!;

    [Column("email")]
    [StringLength(191)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Column("email_verified_at")]
    public DateTime? EmailVerifiedAt { get; set; }

    [Column("password")]
    [StringLength(191)]
    public string Password { get; set; } = null!;

    [Column("remember_token")]
    [StringLength(100)]
    public string? RememberToken { get; set; }
}