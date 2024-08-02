using JG.Code.Catalog.IntegrationTests.Application.UseCases.Category.Common;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class GetCategoryFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture> { }

public class DeleteCategoryTestFixture : CategoryUseCasesBaseFixture
{

}
