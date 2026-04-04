namespace Boom.Common.DTOs.Response;

public class PlayerDto : IPlistSerializable
{
    public int Id { get; set; }
    public Guid Uuid { get; set; }
    public string FacebookId { get; set; }
    public string TwitterId { get; set; }
    public string Nickname { get; set; }
    public string Fullname { get; set; }
    public string Notification { get; set; }
    public string Email { get; set; }
    public int Badge { get; set; }
    public string LastLoginAt { get; set; }
    public string CountryCode { get; set; }
    public string Timezone { get; set; }
    public int TimezoneSecondsOffset { get; set; }
    public int Rev { get; set; }
    public string Device { get; set; }
    public string Ios { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string TinyUrl { get; set; }
    public string HeroStyle { get; set; }
    public string EngineStyle { get; set; }
    public string WheelStyle { get; set; }
    public int TotalHiddenPilesFound { get; set; }
    public int TournamentAggregatedRank { get; set; }

    // Nested profile DTO
    public PlayerProfileDto Profile { get; set; }

    public class PlayerProfileDto : IPlistSerializable
    {
        public string HeroStyle { get; set; }
        public string EngineStyle { get; set; }
        public string WheelStyle { get; set; }
        public string Nickname { get; set; }
        public string CountryCode { get; set; }
        public int TotalEarnedMedals { get; set; }
        public int TotalEarnedSuperstars { get; set; }
        public double TotalDistance { get; set; }

        public ChallengeStatsDto ChallengeStats { get; set; }
        public TournamentStatsDto TournamentStats { get; set; }

        public string MaxGroupIdUnlocked { get; set; }
    }

    public class ChallengeStatsDto : IPlistSerializable
    {
        public int Won { get; set; }
        public int Lost { get; set; }
        public int Played { get; set; }
    }

    public class TournamentStatsDto : IPlistSerializable
    {
        public int Won { get; set; }
        public int Played { get; set; }
        public double AveragePos { get; set; }
    }
}