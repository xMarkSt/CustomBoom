using Boom.Business.Services;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boom.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    // TODO test endpoint remove later
    [HttpPost]
    public async Task<ActionResult<List<TournamentGroup>>> Schedule()
    {
        var currentTournament = await _tournamentService.GetScheduled();

        if (currentTournament.Schedule.Count == 0) return NotFound();
        
        return Ok(currentTournament);
    }
}