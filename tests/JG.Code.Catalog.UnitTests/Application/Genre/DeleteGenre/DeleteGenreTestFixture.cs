using JG.Code.Catalog.UnitTests.Application.Genre.Common;

namespace JG.Code.Catalog.UnitTests.Application.Genre.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection : ICollectionFixture<DeleteGenreTestFixture>
{
}
public class DeleteGenreTestFixture : GenreUseCasesBaseFixture
{
}
