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
        builder.OwnsOne(video => video.Thumb, thumb => thumb.Property(image => image.Path).HasColumnName("ThumbPath"));
        builder.OwnsOne(video => video.ThumbHalf, thumbHalf => thumbHalf.Property(image => image.Path).HasColumnName("ThumbHalfPath"));
        builder.OwnsOne(video => video.Banner, banner => banner.Property(image => image.Path).HasColumnName("BannerPath"));
    }
}
