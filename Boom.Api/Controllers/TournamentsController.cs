using Boom.Api.Filters;
using Boom.Api.Middleware;
using Boom.Business.Services;
using Boom.Common.DTOs.Request;
using Boom.Infrastructure.Data.Entities;
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

    [HttpPost]
    [EncryptResponse]
    public async Task<ActionResult<string>> Join([FromForm] JoinTournamentDto dto)
    {
        // Meta stuff:
        var player = await _playerService.UpdatePlayer(dto);
        
        var result = await _tournamentService.Join(dto, player);

        if (result == null) return NotFound();

        return Ok(result);
    }
    
    [HttpPost]
    [EncryptResponse]
    public async Task<ActionResult<string>> Reload([FromForm] ReloadTournamentDto dto)
    {
        var player = await _playerService.UpdatePlayer(dto);
        var result = await _tournamentService.Reload(dto, player);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TournamentGroup>> Create(
        [FromQuery] int? durationHours,
        [FromQuery] int? durationMinutes)
    {
        if (durationHours == null && durationMinutes == null)
            return BadRequest("Provide either durationHours or durationMinutes.");

        if (durationHours != null && durationMinutes != null)
            return BadRequest("Provide either durationHours or durationMinutes, not both.");

        var duration = durationHours != null
            ? TimeSpan.FromHours(durationHours.Value)
            : TimeSpan.FromMinutes(durationMinutes!.Value);

        var result = await _tournamentService.CreateGroup(duration);

        return Ok(result);
    }
}