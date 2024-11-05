using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.Services;

public interface ITournamentService
{
    Task<TournamentGroup?> GetScheduled();
}