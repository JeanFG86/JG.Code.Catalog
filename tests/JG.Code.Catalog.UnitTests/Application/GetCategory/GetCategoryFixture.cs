using JG.Code.Catalog.UnitTests.Application.Common;

namespace JG.Code.Catalog.UnitTests.Application.GetCategory;

[CollectionDefinition(nameof(GetCategoryFixture))]
public class GetCategoryFixtureCollection : ICollectionFixture<GetCategoryFixture> { }

public class GetCategoryFixture : CategoryUseCasesBaseFixture
{
}
