using JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.Commom;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres;

[CollectionDefinition(nameof(ListGenresTestFixture))]
public class ListGenresTestFixtureCollection : ICollectionFixture<ListGenresTestFixture> { }
public class ListGenresTestFixture : GenreUseCasesBaseFixture
{    
}
