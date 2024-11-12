using JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.Commom;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture> { }
public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
{
}
