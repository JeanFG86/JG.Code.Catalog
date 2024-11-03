using JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.Commom;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection : ICollectionFixture<UpdateGenreTestFixture> { }
public class UpdateGenreTestFixture : GenreUseCasesBaseFixture
{
}
