using JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMembers;

[CollectionDefinition(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTestFixtureCollection : ICollectionFixture<ListCastMembersTestFixture> { }

public class ListCastMembersTestFixture : CastMemberUseCasesBaseFixture
{
    
}