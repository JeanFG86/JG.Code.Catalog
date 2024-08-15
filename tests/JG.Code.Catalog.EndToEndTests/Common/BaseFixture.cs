using Bogus;
using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.EndToEndTests.Common;
public class BaseFixture
{
    protected Faker Faker { get; set; }
    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    public HttpClient HttpClient { get; set; }
    public ApiClient ApiClient { get; set; }

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
    } 

    public CodeCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeCatalogDbContext(new DbContextOptionsBuilder<CodeCatalogDbContext>().UseInMemoryDatabase("end2end-tests-db").Options);
        return dbContext;
    }

    public void CleanPersistence()
    {
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
