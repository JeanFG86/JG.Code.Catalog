using Bogus;
using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.IntegrationTests.Common;
public class BaseFixture
{
    public BaseFixture() => Faker = new Faker("pt_BR");

    protected Faker Faker {  get; set; }

    public CodeCatalogDbContext CreateDbContext(bool preserveData = false, string dbId = "")
    {
        var dbContext = new CodeCatalogDbContext(new DbContextOptionsBuilder<CodeCatalogDbContext>().UseInMemoryDatabase($"integration-tests-db{dbId}").Options);
        if (preserveData == false)
            dbContext.Database.EnsureDeleted();
        return dbContext;
    }

}
