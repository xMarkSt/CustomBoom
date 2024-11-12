using System.Collections;
using System.Reflection;
using AutoMapper;
using Boom.Common;
using Boom.Common.DTOs;
using Boom.Common.Extensions;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Claunia.PropertyList;
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
    public async Task<ScheduleDto> GetScheduled()
    {
        var current = await _repository.GetAll<TournamentGroup>()
            // .Where(x => x.EndsAt > DateTime.Now) // Todo: disabled For testing
            .OrderBy(x => x.EndsAt)
            .FirstOrDefaultAsync();
        return new ScheduleDto()
        {
            Schedule = [_mapper.Map<TournamentGroupDto>(current)]
        };
    }
}