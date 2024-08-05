using JG.Code.Catalog.Application.UseCases.Category.CreateCategory;
using JG.Code.Catalog.EndToEndTests.Api.Category.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTestFixtureCollection : ICollectionFixture<CreateCategoryApiTestFixture>
{}

public class CreateCategoryApiTestFixture : CategoryBaseFixture
{
    public CreateCategoryInput GetExampleInput()
     => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());
}
