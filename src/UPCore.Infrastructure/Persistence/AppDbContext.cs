using Microsoft.EntityFrameworkCore;

using UPCore.Domain.UpEntities;
using UPCore.Domain.UpEntityTemplates;

namespace UPCore.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UpEntity> UpEntities => Set<UpEntity>();

    public DbSet<UpEntityTemplate> UpEntityTemplates => Set<UpEntityTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
