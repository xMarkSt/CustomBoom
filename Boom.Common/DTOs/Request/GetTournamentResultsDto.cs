using Microsoft.AspNetCore.Mvc;

namespace Boom.Common.DTOs.Request
{
    public class GetTournamentResultsDto
    {
        [FromForm(Name = "user_uuid")]
        public Guid UserUuid { get; set; }

        [FromForm(Name = "tournament_uuid")]
        public List<Guid> TournamentUuids { get; set; } = new();
    }
}
