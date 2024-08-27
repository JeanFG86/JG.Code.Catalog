using JG.Code.Catalog.Api.Configurations.Policies;
using JG.Code.Catalog.Api.Filters;

namespace JG.Code.Catalog.Api.Configurations;

public static class ControllersConfigs
{
    public static IServiceCollection AddAndConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(
            options => options.Filters.Add(typeof(ApiGlobalExceptionFilter)))
            .AddJsonOptions(joptions =>
            {
                joptions.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCasePolicy();
            });
        services.AddDocumentation();
        return services;
    }

    private static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static WebApplication UseDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }
}
