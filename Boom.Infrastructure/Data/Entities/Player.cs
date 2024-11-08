using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boom.Infrastructure.Data.Entities;

[Table("players")]
public class Player : EntityBase
{
    [Column("uuid")]
    [StringLength(36)]
    public Guid Uuid { get; set; }

    [Column("facebook_id")]
    public long? FacebookId { get; set; }

    [Column("twitter_id")]
    public long? TwitterId { get; set; }

    [Column("nickname")]
    [StringLength(50)]
    public string Nickname { get; set; }

    [Column("fullname")]
    [StringLength(50)]
    public string Fullname { get; set; }

    [Column("notification")]
    [StringLength(20)]
    public string Notification { get; set; } = null!;

    [Column("email")]
    [StringLength(20)]
    public string Email { get; set; }

    [Column("badge")]
    public int Badge { get; set; }

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("country_code")]
    [StringLength(3)]
    public string CountryCode { get; set; } = null!;

    [Column("timezone")]
    [StringLength(50)]
    public string Timezone { get; set; } = null!;

    [Column("timezone_seconds_offset")]
    public int TimezoneSecondsOffset { get; set; }

    [Column("rev")]
    public int? Rev { get; set; }

    [Column("device")]
    [StringLength(30)]
    public string Device { get; set; }

    [Column("ios")]
    [StringLength(20)]
    public string Ios { get; set; }

    [Column("tiny_url")]
    [StringLength(20)]
    public string TinyUrl { get; set; }

    [Column("hero_style")]
    [StringLength(20)]
    public string HeroStyle { get; set; } = null!;

    [Column("engine_style")]
    [StringLength(20)]
    public string EngineStyle { get; set; } = null!;

    [Column("wheel_style")]
    [StringLength(20)]
    public string WheelStyle { get; set; } = null!;

    [Column("total_hidden_piles_found")]
    public int? TotalHiddenPilesFound { get; set; }

    [Column("total_earned_medals")]
    public int TotalEarnedMedals { get; set; }

    [Column("total_earned_superstars")]
    public int TotalEarnedSuperstars { get; set; }

    [Column("total_distance")]
    public double TotalDistance { get; set; }

    [Column("vs_played")]
    public int VsPlayed { get; set; } = 0;

    [Column("vs_won")]
    public int VsWon { get; set; } = 0;

    [Column("wc_played")]
    public int WcPlayed { get; set; } = 0;

    [Column("wc_won")]
    public int WcWon { get; set; } = 0;

    [Column("tournaments_aggregated_rank")]
    public int TournamentsAggregatedRank { get; set; } = 0;

    [Column("world_rank")]
    public int? WorldRank { get; set; }

    [Column("max_group_id_unlocked")]
    [StringLength(50)]
    public string MaxGroupIdUnlocked { get; set; } = null!;

    [Column("secret_key")]
    [StringLength(25)]
    public string SecretKey { get; set; }
}