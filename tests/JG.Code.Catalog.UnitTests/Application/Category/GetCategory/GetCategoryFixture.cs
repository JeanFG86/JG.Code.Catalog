using JG.Code.Catalog.UnitTests.Application.Category.Common;

namespace JG.Code.Catalog.UnitTests.Application.Category.GetCategory;

[CollectionDefinition(nameof(GetCategoryFixture))]
public class GetCategoryFixtureCollection : ICollectionFixture<GetCategoryFixture> { }

public class GetCategoryFixture : CategoryUseCasesBaseFixture
{
}
