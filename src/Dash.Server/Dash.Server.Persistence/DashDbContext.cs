using Dash.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dash.Server.Persistence;

public sealed class DashDbContext : DbContext
{
    public DashDbContext(DbContextOptions<DashDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Page> Pages => Set<Page>();

    public DbSet<Widget> Widgets => Set<Widget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DashDbContext).Assembly);
    }
}
