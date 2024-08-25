using JG.Code.Catalog.Api.ApiModels.Category;
using JG.Code.Catalog.EndToEndTests.Api.Category.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture>
{ }

public class UpdateCategoryApiTestFixture : CategoryBaseFixture
{
    public UpdateCategoryApiInput GetExampleInput()
    {
        return new UpdateCategoryApiInput(GetValidCategoryName() , GetValidCategoryDescription(), GetRandonBoolean());        
    }
}
