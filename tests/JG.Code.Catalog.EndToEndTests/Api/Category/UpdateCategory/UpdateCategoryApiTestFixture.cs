using JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;
using JG.Code.Catalog.EndToEndTests.Api.Category.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture>
{ }

public class UpdateCategoryApiTestFixture : CategoryBaseFixture
{
    public UpdateCategoryInput GetExampleInput(Guid? id = null)
    {
        return new UpdateCategoryInput(id ?? new Guid(), GetValidCategoryName() , GetValidCategoryDescription(), GetRandonBoolean());        
    }
}
