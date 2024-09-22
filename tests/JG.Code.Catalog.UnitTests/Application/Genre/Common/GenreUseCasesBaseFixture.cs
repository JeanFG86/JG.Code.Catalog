using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.UnitTests.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCasesBaseFixture : BaseFixture
{
    public string GetValidGenreName() =>
       Faker.Commerce.Categories(1)[0];

    public DomainEntity.Genre GetExampleGenre(bool? isActive = null) =>
       new(GetValidGenreName(), isActive ?? GetRandomBoolean());

    public Mock<IGenreRepository> GetGenreRepositoryMock() => new();
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}
