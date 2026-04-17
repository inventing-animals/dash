using Dash.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dash.Server.Persistence;

public sealed class DashDbContext : DbContext
{
    public DashDbContext(DbContextOptions<DashDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User> ();

    public DbSet<Page> Pages => Set<Page>();

    public DbSet<Widget> Widgets => Set<Widget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DashDbContext).Assembly);

        if (Database.IsSqlite())
        {
            ApplySqliteGuidConverters(modelBuilder);
        }
    }

    private static void ApplySqliteGuidConverters(ModelBuilder modelBuilder)
    {
        var guidConverter = new GuidToStringConverter();
        var nullableGuidConverter = new ValueConverter<Guid?, string?>(
            value => value.HasValue ? value.Value.ToString() : null,
            value => string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(Guid))
                {
                    property.SetValueConverter(guidConverter);
                    property.SetColumnType("TEXT");
                }
                else if (property.ClrType == typeof(Guid?))
                {
                    property.SetValueConverter(nullableGuidConverter);
                    property.SetColumnType("TEXT");
                }
            }
        }
    }
}
