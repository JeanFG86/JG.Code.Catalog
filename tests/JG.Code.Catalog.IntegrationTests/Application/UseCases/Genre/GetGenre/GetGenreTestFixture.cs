using JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.Commom;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture> { }
public class GetGenreTestFixture : GenreUseCasesBaseFixture
{
}
