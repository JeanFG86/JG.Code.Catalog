using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JG.Code.Catalog.Infra.Data.EF.Configurations;

public class VideosCategoriesConfiguration : IEntityTypeConfiguration<VideosCategories>
{
    public void Configure(EntityTypeBuilder<VideosCategories> builder)
    {
        builder.HasKey(relation => new { relation.CategoryId, relation.VideoId });
    }
}