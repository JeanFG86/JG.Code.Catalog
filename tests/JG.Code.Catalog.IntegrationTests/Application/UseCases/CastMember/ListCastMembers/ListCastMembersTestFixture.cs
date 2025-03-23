using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMembers;

[CollectionDefinition(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTestFixtureCollection : ICollectionFixture<ListCastMembersTestFixture> { }

public class ListCastMembersTestFixture : CastMemberUseCasesBaseFixture
{
    public List<DomainEntity.CastMember> GetExampleCastMembersListWithNames(List<string> names)
        => names.Select(name =>
        {
            var castMember = GetExampleCastMember();
            castMember.Update(name, castMember.Type);
            return castMember;
        }).ToList();
}
