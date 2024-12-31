using Boom.Common.DTOs.Request;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boom.Business.Services;

public class PlayerService : IPlayerService
{
    private readonly IRepository _repository;
    private readonly IEncryptionService _encryptionService;

    public PlayerService(IRepository repository, IEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public async Task UpdatePlayer(GetScheduleDto dto)
    {
        var player = await _repository.GetAll<Player>().FirstOrDefaultAsync(p => p.Uuid == dto.user_uuid);

        if (player == null)
        {
            // Create player
            // TODO: fill more values here (automapper?)
            player = new Player
            {
                Uuid = dto.user_uuid,
                SecretKey = _encryptionService.GenerateSecretKey()
            };
            await _repository.CreateAsync(player);
        }
        
        // Update player
        // TODO: fill values here (automapper?)
        await _repository.UpdateAsync(player);
    }
}