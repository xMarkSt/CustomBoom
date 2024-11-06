using Boom.Common.DTOs;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Claunia.PropertyList;
using Microsoft.EntityFrameworkCore;

namespace Boom.Business.Services;

public class TournamentService : ITournamentService
{
    // todo move to service
    private readonly BoomDbContext _context;

    public TournamentService(BoomDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Get the currently scheduled tournament group
    /// </summary>
    /// <returns></returns>
    public async Task<TournamentGroup?> GetScheduled()
    {
        return await _context.TournamentGroups
            .Where(x => x.EndsAt > DateTime.Now)
            .OrderBy(x => x.EndsAt)
            .FirstOrDefaultAsync();
    }
    
    public NSObject SerializeTest(ScheduleDto dto)
    {
        var a = NSObject.Wrap(dto.Schedule.First().Level);
        return a;
    }
}