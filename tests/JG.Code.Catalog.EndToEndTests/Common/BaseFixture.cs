using Bogus;
using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.EndToEndTests.Common;
public class BaseFixture
{
    protected Faker Faker { get; set; }

    public ApiClient ApiClient { get; set; }

    public BaseFixture() => Faker = new Faker("pt_BR");

    public CodeCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeCatalogDbContext(new DbContextOptionsBuilder<CodeCatalogDbContext>().UseInMemoryDatabase("end2end-tests-db").Options);
        return dbContext;
    }
}
