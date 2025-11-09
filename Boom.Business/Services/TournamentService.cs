using AutoMapper;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boom.Business.Services;

public class TournamentService : ITournamentService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    public TournamentService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Get the currently scheduled tournament group
    /// </summary>
    /// <returns></returns>
    public async Task<ScheduleDto> GetSchedule()
    {
        var current = await _repository.GetAll<TournamentGroup>()
            .Where(x => x.EndsAt > DateTime.Now)
            .Include(x => x.LevelTarget.Level)
            .Include(x => x.LevelTarget.Level.Theme)
            .Include(x => x.LevelTarget.Level.Background)
            .Include(x => x.LevelTarget.Target)
            .OrderBy(x => x.EndsAt)
            .FirstOrDefaultAsync();
        
        return new ScheduleDto
        {
            Schedule = current != null ? [_mapper.Map<TournamentGroupDto>(current)] : []
        };
    }
}