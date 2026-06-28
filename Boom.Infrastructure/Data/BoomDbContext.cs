using Boom.Infrastructure.Data.Entities;
using Boom.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     => optionsBuilder
    //         .UseAsyncSeeding(async (context, _, cancellationToken) =>
    //         {
    //             SeedHelper.SeedLevels(context, cancellationToken);
    //         });
}