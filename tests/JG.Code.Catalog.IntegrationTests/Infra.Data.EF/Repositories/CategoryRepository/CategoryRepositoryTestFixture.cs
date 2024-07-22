using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTestFixtureCollection : ICollectionFixture<CategoryRepositoryTestFixture>
{

}

public class CategoryRepositoryTestFixture : BaseFixture
{
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

    public List<Category> GetExampleCategoriesListWithNames(List<string> names) 
        => names.Select(name => 
        { 
            var category = GetExampleCategory();
            category.Update(name);
            return category;
        }).ToList();

    public CodeCatalogDbContext CreateDbContext(bool preserveData = false)
    {
        var dbContext = new CodeCatalogDbContext(new DbContextOptionsBuilder<CodeCatalogDbContext>().UseInMemoryDatabase("integration-tests-db").Options);
        if(preserveData == false)
            dbContext.Database.EnsureDeleted();
        return dbContext;

    }
}
