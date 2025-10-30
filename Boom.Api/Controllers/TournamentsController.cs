using System.Text;
using Boom.Api.Middleware;
using Boom.Business.Services;
using Boom.Common;
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

    // TODO: make plist result
    [HttpPost]
    public async Task<ActionResult<string>> Schedule([FromForm] GetScheduleDto dto)
    {
        // Meta stuff:
        var player = await _playerService.UpdatePlayer(dto);
        var requestEncrypted = HttpContext.Items.TryGetValue(RequestDecryptionMiddleware.RequestEncryptedItemKey, out var item)
            && item is bool encrypted && encrypted;

        // check secret key
        // if (!$request->input('requestEncrypted')) {
        //     if (!empty($player->secretKey)) {
        //         $dict->add('_sk', new CFString($player->secretKey));
        //     }
        // }
        // IF: request not encrypted and secret key not empty
        // THEN: add secret key to response
        
        // Actual tournament stuff:

        var currentTournament = await _tournamentService.GetSchedule();

        if (!requestEncrypted && !string.IsNullOrEmpty(player.SecretKey))
        {
            currentTournament.SecretKey = player.SecretKey;
        }

        if (currentTournament.Schedule.Count == 0) return NotFound();

        return PlistResult(currentTournament);
    }

    private ActionResult PlistResult(IPlistSerializable dto)
    {
        // TODO: use encryption if enabled in settings
        // return _encryptionService.Encrypt(_plistService.ToPlistString(dto));
        return File(
            Encoding.UTF8.GetBytes(_plistService.ToPlistString(dto)),
            "application/x-plist");
    }
}