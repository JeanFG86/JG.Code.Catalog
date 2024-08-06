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
        builder.ConfigureServices(services =>
        {
            var dbOptions = services.FirstOrDefault(x => x.ServiceType == typeof(DbContextOptions<CodeCatalogDbContext>));
            if(dbOptions != null)            
                services.Remove(dbOptions);
            services.AddDbContext<CodeCatalogDbContext>(options =>
            {
                options.UseInMemoryDatabase("end2end-tests-db");
            });
        });
        base.ConfigureWebHost(builder);
    }
}
