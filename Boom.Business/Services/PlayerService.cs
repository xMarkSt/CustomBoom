using AutoMapper;
using Boom.Common.DTOs.Request;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boom.Business.Services;

public class PlayerService : IPlayerService
{
    private readonly IRepository _repository;
    private readonly IEncryptionService _encryptionService;
    private readonly IMapper _mapper;

    public PlayerService(IRepository repository, IEncryptionService encryptionService, IMapper mapper)
    {
        _repository = repository;
        _encryptionService = encryptionService;
        _mapper = mapper;
    }

    private async Task CreatePlayer(GetScheduleDto dto)
    {
        // Create player
        var player = _mapper.Map<Player>(dto);
        player.SecretKey = _encryptionService.GenerateSecretKey();
        await _repository.CreateAsync(player);
    }

    public async Task UpdatePlayer(GetScheduleDto dto)
    {
        var player = await _repository.GetAll<Player>().FirstOrDefaultAsync(p => p.Uuid == dto.user_uuid);

        // Uuid sent by client not in database yet, create the new player.
        if (player == null)
        {
            await CreatePlayer(dto);
        }
        
        // Update existing player
        else
        {
            await _repository.UpdateAsync(player);
        }
    }
}