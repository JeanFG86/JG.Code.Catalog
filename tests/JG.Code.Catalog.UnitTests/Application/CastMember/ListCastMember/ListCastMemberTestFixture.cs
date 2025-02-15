using JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.UnitTests.Application.CastMember.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.ListCastMember;

[CollectionDefinition(nameof(ListCastMemberTestFixture))]
public class ListCastMemberTestFixtureCollection : ICollectionFixture<ListCastMemberTestFixture> { }

public class ListCastMemberTestFixture : CastMemberUseCasesBaseFixture
{
    public List<DomainEntity.CastMember> GetExampleCastMemberList(int length = 10)
    {
        var castMembers = new List<DomainEntity.CastMember>();
        for (int i = 0; i < length; i++)
            castMembers.Add(GetExampleCastMember());
        return castMembers;
    }
    
    public ListCastMembersInput GetExampleInput()
    {
        var random = new Random();
        return new ListCastMembersInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
    }
}