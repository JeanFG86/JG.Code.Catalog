using Bogus;
using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JG.Code.Catalog.EndToEndTests.Common;
public class BaseFixture
{
    protected Faker Faker { get; set; }
    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    public HttpClient HttpClient { get; set; }
    public ApiClient ApiClient { get; set; }

    private readonly string _dbConnectionString;

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
        var configuration = WebAppFactory.Services.GetService(typeof(IConfiguration));
        ArgumentNullException.ThrowIfNull(configuration);
        _dbConnectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb");
    }

    public CodeCatalogDbContext CreateDbContext()
    {
        var context = new CodeCatalogDbContext(new DbContextOptionsBuilder<CodeCatalogDbContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options);
        return context;
    }

    public void CleanPersistence()
    {
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
