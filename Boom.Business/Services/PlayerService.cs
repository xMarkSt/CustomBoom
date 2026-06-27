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
    
    private async Task<Player> CreatePlayer(IPlayerInfo playerInfo)
    {
        // Create player
        var player = _mapper.Map<Player>(playerInfo);
        player.SecretKey = _encryptionService.GenerateSecretKey();
        _repository.Add(player);
        await _repository.SaveAsync();
        return player;
    }

    public async Task<Player> UpdatePlayer(IPlayerInfo playerInfo)
    {
        var player = await _repository.GetAll<Player>().FirstOrDefaultAsync(p => p.Uuid == playerInfo.UserUuid);

        // Uuid sent by client not in database yet, create the new player.
        if (player == null)
        {
            return await CreatePlayer(playerInfo);
        }
        
        // Update existing player
        _mapper.Map(playerInfo, player, playerInfo.GetType(), typeof(Player));
        _repository.Update(player);
        await _repository.SaveAsync();
        return player;
    }
}