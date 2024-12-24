using JG.Code.Catalog.EndToEndTests.Api.Genre.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.ListGenres;

[CollectionDefinition(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTestFixtureCollection : ICollectionFixture<ListGenresApiTestFixture>
{ }
public class ListGenresApiTestFixture: GenreBaseFixture
{
    
}