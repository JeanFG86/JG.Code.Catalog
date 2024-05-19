namespace JG.Code.Catalog.UnitTests.Domain.Entity.Category;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
public class CategoryTestFixture
{
    public DomainEntity.Category GetValidCategory() 
        => new("Category name", "Category description");
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection : ICollectionFixture<CategoryTestFixture> { }
