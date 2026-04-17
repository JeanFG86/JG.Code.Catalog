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
        builder.OwnsOne(video => video.Media, media =>
        {
            media.Ignore(m => m.Id);
            media.Property(m => m.FilePath).HasColumnName("MediaFilePath");
            media.Property(m => m.EncodedPath).HasColumnName("MediaEncodedPath").IsRequired(false);
            media.Property(m => m.Status).HasColumnName("MediaStatus");
        });
        builder.OwnsOne(video => video.Trailer, trailer =>
        {
            trailer.Ignore(m => m.Id);
            trailer.Property(m => m.FilePath).HasColumnName("TrailerFilePath");
            trailer.Property(m => m.EncodedPath).HasColumnName("TrailerEncodedPath").IsRequired(false);
            trailer.Property(m => m.Status).HasColumnName("TrailerStatus");
        });
    }
}
