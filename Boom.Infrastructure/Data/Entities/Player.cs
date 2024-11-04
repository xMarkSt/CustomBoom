using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("players")]
public class Player
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [StringLength(36)] public Guid Uuid { get; set; }

    public long? FacebookId { get; set; }

    public long? TwitterId { get; set; }

    [StringLength(50)] public string? Nickname { get; set; }

    [StringLength(50)] public string? Fullname { get; set; }

    [StringLength(20)] public string Notification { get; set; } = null!;

    [StringLength(20)] public string? Email { get; set; }

    public int Badge { get; set; }

    public DateTime? LastLoginAt { get; set; }

    [StringLength(3)] public string CountryCode { get; set; } = null!;

    [StringLength(50)] public string Timezone { get; set; } = null!;

    public int TimezoneSecondsOffset { get; set; }

    public int? Rev { get; set; }

    [StringLength(30)] public string? Device { get; set; }

    [StringLength(20)] public string? Ios { get; set; }

    [StringLength(20)] public string? TinyUrl { get; set; }

    [StringLength(20)] public string HeroStyle { get; set; } = null!;

    [StringLength(20)] public string EngineStyle { get; set; } = null!;

    [StringLength(20)] public string WheelStyle { get; set; } = null!;

    public int? TotalHiddenPilesFound { get; set; }

    public int TotalEarnedMedals { get; set; }

    public int TotalEarnedSuperstars { get; set; }

    public double TotalDistance { get; set; }

    public int VsPlayed { get; set; } = 0;

    public int VsWon { get; set; } = 0;

    public int WcPlayed { get; set; } = 0;

    public int WcWon { get; set; } = 0;

    public int TournamentsAggregatedRank { get; set; } = 0;

    public int? WorldRank { get; set; }

    [StringLength(50)] public string MaxGroupIdUnlocked { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(25)] public string? SecretKey { get; set; }
}