using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boom.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TournamentsController : ControllerBase
{
    // todo move to service
    private readonly BoomDbContext _context;

    public TournamentsController(BoomDbContext context)
    {
        _context = context;
    }

    // TODO test endpoint remove later
    [HttpGet("get")]
    public async Task<ActionResult<List<TournamentGroup>>> Get()
    {
        var todoItem = await _context.TournamentGroups.ToListAsync();

        return todoItem;
    }
}