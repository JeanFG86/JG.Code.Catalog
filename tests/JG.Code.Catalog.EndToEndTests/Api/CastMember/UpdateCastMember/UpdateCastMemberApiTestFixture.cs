using JG.Code.Catalog.Api.ApiModels.CastMember;
using JG.Code.Catalog.EndToEndTests.Api.CastMember.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember;

[CollectionDefinition(nameof(UpdateCastMemberApiTestFixture))]
public class UpdateCastMemberApiTestFixtureCollection : ICollectionFixture<UpdateCastMemberApiTestFixture>
{ }

public class UpdateCastMemberApiTestFixture : CastMemberApiBaseFixture
{
    public UpdateCastMemberApiInput GetExampleInput()
    {
        return new UpdateCastMemberApiInput(GetValidName() , GetRandomCastMemberType());        
    }
}