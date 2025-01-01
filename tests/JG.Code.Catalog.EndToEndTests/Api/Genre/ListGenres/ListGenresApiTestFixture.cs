using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
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
    
    public List<DomainEntity.Genre> CloneGenresListOrdered(List<DomainEntity.Genre> genresList, string orderBy, SearchOrder searchOrder)
    {
        var listClone = new List<DomainEntity.Genre>(genresList);
        var orderedEnumerable = (orderBy, searchOrder) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(n => n.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(n => n.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(n => n.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(n => n.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(n => n.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(n => n.CreatedAt),
            _ => listClone.OrderBy(n => n.Name).ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}