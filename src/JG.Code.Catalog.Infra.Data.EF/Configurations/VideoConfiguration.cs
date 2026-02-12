using JG.Code.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JG.Code.Catalog.Infra.Data.EF.Configurations;

internal class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Title).HasMaxLength(255);
        builder.Property(category => category.Description).HasMaxLength(4_000);
    }
}
