using System.Text;
using Boom.Business.Services;
using Boom.Common;
using Microsoft.AspNetCore.Mvc;

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

        return PlistResult(currentTournament);
    }

    private ActionResult PlistResult(IPlistSerializable dto)
    {
        // TODO: if production, use encryption.
        return File(
            Encoding.UTF8.GetBytes(_plistService.ToPlistString(dto)),
            "application/x-plist");
    }
}