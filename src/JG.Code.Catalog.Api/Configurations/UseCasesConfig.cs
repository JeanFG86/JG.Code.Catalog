using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.Category.CreateCategory;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using JG.Code.Catalog.Infra.Data.EF;

namespace JG.Code.Catalog.Api.Configurations;

public static class UseCasesConfig
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateCategory).Assembly));
        services.AddRepositories();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
