using JG.Code.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using JG.Code.Catalog.UnitTests.Application.CastMember.Common;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.UpdateCastMember;


[CollectionDefinition(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTestFixtureCollection : ICollectionFixture<UpdateCastMemberTestFixture>
{
}

public class UpdateCastMemberTestFixture: CastMemberUseCasesBaseFixture
{
    public UpdateCastMemberInput GetValidInput(Guid? id = null)
        => new UpdateCastMemberInput(id ?? Guid.NewGuid(), GetValidName(), GetRandomCastMemberType());
}