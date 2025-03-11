using JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;
using JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.CreateCastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection : ICollectionFixture<CreateCastMemberTestFixture>
{
}

public class CreateCastMemberTestFixture : CastMemberUseCasesBaseFixture
{
    public CreateCastMemberInput GetInput()
    {
        var castMember = GetExampleCastMember();
        return new CreateCastMemberInput(castMember.Name, castMember.Type);
    }
}