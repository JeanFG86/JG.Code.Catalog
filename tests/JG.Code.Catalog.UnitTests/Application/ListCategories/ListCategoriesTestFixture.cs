using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.UnitTests.Common;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.ListCategories;


[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture> { }

public class ListCategoriesTestFixture : BaseFixture
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
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public bool GetRandomBoolean() => (new Random()).NextDouble() < 0.5;

    public Category GetExampleCategory()
        => new Category(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public List<Category> GetExampleCategoriesList(int length = 10)
    {
        var categories = new List<Category>();
        for (int i = 0; i < length; i++)        
            categories.Add(GetExampleCategory());        
        return categories;
    }

    public Mock<ICategoryRepository> GetRepositoryMock() => new();
}
