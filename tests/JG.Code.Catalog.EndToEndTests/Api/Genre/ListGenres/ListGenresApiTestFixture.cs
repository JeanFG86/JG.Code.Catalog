using JG.Code.Catalog.EndToEndTests.Api.Genre.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.ListGenres;

[CollectionDefinition(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTestFixtureCollection : ICollectionFixture<ListGenresApiTestFixture>
{ }
public class ListGenresApiTestFixture: GenreBaseFixture
{
    public List<DomainEntity.Genre> GetExampleListGenresByNames(List<string> names)
    {
        return names.Select(n => GetExampleGenre(name: n)).ToList();
    }
}