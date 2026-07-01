using Microsoft.AspNetCore.Mvc;

namespace Boom.Common.DTOs.Request
{
    public class GhostTournamentDto
    {
        [FromForm(Name = "tournament_uuid")]
        public Guid TournamentUuid { get; set; }

        [FromForm(Name = "opponent_uuid")]
        public Guid OpponentUuid { get; set; }
    }
}
