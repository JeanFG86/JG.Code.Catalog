using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Infra.Data.EF.Configurations;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.Infra.Data.EF;
public class CodeCatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public CodeCatalogDbContext(DbContextOptions<CodeCatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        //base.OnModelCreating(modelBuilder);
    }
}
