﻿using JG.Code.Catalog.Domain.Entity;
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

    public CodeCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeCatalogDbContext(new DbContextOptionsBuilder<CodeCatalogDbContext>().UseInMemoryDatabase("integration-tests-db").Options)
    }
}