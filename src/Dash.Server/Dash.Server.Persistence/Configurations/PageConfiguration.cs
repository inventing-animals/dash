using Dash.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dash.Server.Persistence.Configurations;

public sealed class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Pages");

        builder.HasKey(page => page.PageId);

        builder.Property(page => page.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(page => new { page.UserId, page.Name });

        builder.HasMany(page => page.Widgets)
            .WithOne(widget => widget.Page)
            .HasForeignKey(widget => widget.PageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
