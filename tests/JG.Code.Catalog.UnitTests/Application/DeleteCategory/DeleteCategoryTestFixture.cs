using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.UnitTests.Application.Common;
using JG.Code.Catalog.UnitTests.Common;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class GetCategoryFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture> { }

public class DeleteCategoryTestFixture : CategoryUseCasesBaseFixture
{
    public Category GetValidCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription());
}
