using JG.Code.Catalog.IntegrationTests.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.Commom;
public class GenreUseCasesBaseFixture : BaseFixture
{
    public string GetValidGenreName() =>
       Faker.Commerce.Categories(1)[0];

    public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string? name = null)
    {
        var genre = new DomainEntity.Genre(name ?? GetValidGenreName(), isActive ?? GetRandomBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<DomainEntity.Genre> GetExampleListGenres(int count = 10)
    {
        return Enumerable.Range(1, count).Select(_ => GetExampleGenre()).ToList();
    }

    public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;
}
