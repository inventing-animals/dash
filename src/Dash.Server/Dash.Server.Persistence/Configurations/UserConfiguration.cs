using Dash.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dash.Server.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.UserId);

        builder.Property(user => user.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasMany(user => user.Pages)
            .WithOne(page => page.User)
            .HasForeignKey(page => page.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
