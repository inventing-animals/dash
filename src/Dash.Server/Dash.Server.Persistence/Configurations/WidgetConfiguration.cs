using Dash.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dash.Server.Persistence.Configurations;

public sealed class WidgetConfiguration : IEntityTypeConfiguration<Widget>
{
    public void Configure(EntityTypeBuilder<Widget> builder)
    {
        builder.ToTable("Widgets");

        builder.HasKey(widget => widget.WidgetId);

        builder.Property(widget => widget.WidgetType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(widget => widget.ConfigurationJson)
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(widget => widget.StateJson)
            .HasColumnType("TEXT")
            .IsRequired(false);

        builder.HasIndex(widget => new { widget.PageId, widget.WidgetType });
    }
}
