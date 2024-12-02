using JG.Code.Catalog.EndToEndTests.Api.Category.Common;
using JG.Code.Catalog.EndToEndTests.Common;
using JG.Code.Catalog.Infra.Data.EF;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.Common;

public class GenreBaseFixture : BaseFixture
{
    public GenrePersistence Persistence { get; set; }
    public CategoryPersistence CategoryPersistence { get; set; }

    public GenreBaseFixture() : base()
    {
        CodeCatalogDbContext dbContext = CreateDbContext();
        Persistence = new GenrePersistence(dbContext);
        CategoryPersistence = new CategoryPersistence(dbContext);
    }
    
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

    public DomainEntity.Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
}