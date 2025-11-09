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
    private readonly IPlistSerializationService _plistService;
    private readonly IEncryptionService _encryptionService;
    private readonly IPlayerService _playerService;

    public TournamentsController(
        ITournamentService tournamentService,
        IPlistSerializationService plistService,
        IEncryptionService encryptionService, IPlayerService playerService)
    {
        _tournamentService = tournamentService;
        _plistService = plistService;
        _encryptionService = encryptionService;
        _playerService = playerService;
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
        if (!requestEncrypted && !string.IsNullOrEmpty(player.SecretKey))
        {
            currentTournament.SecretKey = player.SecretKey;
        }

        if (currentTournament.Schedule.Count == 0) return NotFound();

        return Ok(currentTournament);
    }
}