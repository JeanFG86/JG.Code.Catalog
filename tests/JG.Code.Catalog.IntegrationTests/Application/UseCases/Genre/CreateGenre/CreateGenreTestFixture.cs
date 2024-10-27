using JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
using JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.Commom;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }
public class CreateGenreTestFixture : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput()
    {
        return new CreateGenreInput(GetValidGenreName(), GetRandomBoolean());
    }
}
