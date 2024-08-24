using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbConnection(configuration);
        return services;
    }

    private static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDb");
        services.AddDbContext<CodeCatalogDbContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );
        return services;
    }
}
