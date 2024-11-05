using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
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
}