using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.IntegrationTests.Common;

namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;

[CollectionDefinition(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTestFixtureCollection : ICollectionFixture<CastMemberRepositoryTestFixture>
{

}

public class CastMemberRepositoryTestFixture : BaseFixture
{
    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1,2);

    public CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMemberType());
    
    public List<CastMember> GetExampleCastMembersList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCastMember()).ToList();
    
    public List<CastMember> GetExampleCastMembersListWithNames(List<string> names) 
        => names.Select(name => 
        { 
            var castMember = GetExampleCastMember();
            castMember.Update(name, castMember.Type);
            return castMember;
        }).ToList();
    
    public List<CastMember> CloneCastMembersListOrdered(List<CastMember> castMembersList, string orderBy, SearchOrder searchOrder)
    {
        var listClone = new List<CastMember>(castMembersList);
        var orderedEnumerable = (orderBy, searchOrder) switch
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