using Boom.Business.Services;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boom.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly IPlistSerializationService _plistService;

    public TournamentsController(
        ITournamentService tournamentService,
        IPlistSerializationService plistService
    )
    {
        _tournamentService = tournamentService;
        _plistService = plistService;
    }

    // TODO: make plist result
    [HttpPost]
    public async Task<ActionResult<string>> Schedule()
    {
        var currentTournament = await _tournamentService.GetSchedule();

        if (currentTournament.Schedule.Count == 0) return NotFound();

        return Ok(_plistService.ToPlistString(currentTournament));
    }
}