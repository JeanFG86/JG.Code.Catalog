using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.UnitTests.Common;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCasesBaseFixture : BaseFixture
{
    public string GetValidGenreName() =>
       Faker.Commerce.Categories(1)[0];

    public Mock<IGenreRepository> GetRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}
