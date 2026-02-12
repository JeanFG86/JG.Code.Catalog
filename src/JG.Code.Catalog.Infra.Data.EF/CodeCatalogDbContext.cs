using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Infra.Data.EF.Configurations;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.Infra.Data.EF;
public class CodeCatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<GenresCategories> GenresCategories => Set<GenresCategories>();
    public CodeCatalogDbContext(DbContextOptions<CodeCatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new CastMemberConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new GenresCategoriesConfiguration());
        modelBuilder.ApplyConfiguration(new VideoConfiguration());
    }
}
