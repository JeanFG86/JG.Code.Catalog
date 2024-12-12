using JG.Code.Catalog.EndToEndTests.Api.Genre.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTestFixtureCollection : ICollectionFixture<CreateGenreApiTestFixture>
{ }

public class CreateGenreApiTestFixture : GenreBaseFixture
{
    
}