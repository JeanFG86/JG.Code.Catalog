using JG.Code.Catalog.EndToEndTests.Api.Category.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryApiTestFixture))]
public class DeleteCategoryApiTestFixtureCollection : ICollectionFixture<DeleteCategoryApiTestFixture>
{ }

public class DeleteCategoryApiTestFixture : CategoryBaseFixture
{
}
