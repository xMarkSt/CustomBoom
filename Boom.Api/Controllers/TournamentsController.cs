using Boom.Api.Filters;
using Boom.Api.Middleware;
using Boom.Business.Services;
using Boom.Common.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace Boom.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly IPlayerService _playerService;
    private readonly IConfiguration _configuration;

    public TournamentsController(
        ITournamentService tournamentService, IPlayerService playerService, IConfiguration configuration)
    {
        _tournamentService = tournamentService;
        _playerService = playerService;
        _configuration = configuration;
    }

    [HttpPost]
    [EncryptResponse]
    public async Task<ActionResult<string>> Schedule([FromForm] GetScheduleDto dto)
    {
        // Meta stuff:
        var player = await _playerService.UpdatePlayer(dto);

        // check secret key
        var requestEncrypted =
            HttpContext.Items.TryGetValue(RequestDecryptionMiddleware.RequestEncryptedItemKey, out var item)
            && item is true;

        // Actual tournament stuff:
        var currentTournament = await _tournamentService.GetSchedule();

        // If not encrypted, add the secret key to make the requests encrypted
        if (!_configuration.GetValue<bool>("DisableEncryption") && !requestEncrypted && !string.IsNullOrEmpty(player.SecretKey))
        {
            currentTournament.SecretKey = player.SecretKey;
        }

        if (currentTournament.Schedule.Count == 0) return NotFound();

        return Ok(currentTournament);
    }
}