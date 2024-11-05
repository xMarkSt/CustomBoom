using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boom.Infrastructure.Data;

public class BoomDbContext(DbContextOptions<BoomDbContext> options) : DbContext(options)
{
    // Entities        
    public DbSet<TournamentGroup> TournamentGroups { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<Article> Articles { get; set; } = null!;
    public DbSet<Ghost> Ghosts { get; set; } = null!;
    public DbSet<Level> Levels { get; set; } = null!;
    public DbSet<LevelTarget> LevelTargets { get; set; } = null!;
    public DbSet<Target> Targets { get; set; } = null!;
    public DbSet<Standing> Standings { get; set; } = null!;
    public DbSet<Theme> Themes { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}