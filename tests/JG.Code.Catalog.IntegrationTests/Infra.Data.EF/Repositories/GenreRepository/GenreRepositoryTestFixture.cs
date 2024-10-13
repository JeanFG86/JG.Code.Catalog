using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.IntegrationTests.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture>
{

}

public class GenreRepositoryTestFixture : BaseFixture
{
    public string GetValidGenreName() =>
       Faker.Commerce.Categories(1)[0];

    public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        var genre = new Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = "";
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

    public Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
}
