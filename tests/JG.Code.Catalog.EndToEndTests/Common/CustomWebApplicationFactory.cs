using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JG.Code.Catalog.EndToEndTests.Common;

//permite rodar a api no proprio processo do teste (WebApplicationFactory)
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> 
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("EndToEndTest");
        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<CodeCatalogDbContext>();
            ArgumentNullException.ThrowIfNull(context);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });
        base.ConfigureWebHost(builder);
    }
}
