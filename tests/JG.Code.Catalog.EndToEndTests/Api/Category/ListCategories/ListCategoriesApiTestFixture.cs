using JG.Code.Catalog.EndToEndTests.Api.Category.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.ListCategories;

[CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTestFixtureCollection : ICollectionFixture<ListCategoriesApiTestFixture> { }

public class ListCategoriesApiTestFixture : CategoryBaseFixture
{
}
