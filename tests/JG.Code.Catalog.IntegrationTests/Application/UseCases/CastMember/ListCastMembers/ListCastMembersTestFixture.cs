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

public List<DomainEntity.CastMember> CloneCastMembersListOrdered(List<DomainEntity.CastMember> castMembersList, string orderBy, SearchOrder order)
    {
        var listClone = new List<DomainEntity.CastMember>(castMembersList);
        var orderedEnumerable = (orderBy, order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(n => n.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(n => n.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(n => n.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(n => n.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(n => n.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(n => n.CreatedAt),
            _ => listClone.OrderBy(n => n.Name).ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}