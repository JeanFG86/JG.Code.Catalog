using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.EndToEndTests.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.Common;

public class CastMemberBaseFixture : BaseFixture
{
    public CastMemberPersistence Persistence;

    public CastMemberBaseFixture() : base()
    {
        Persistence = new CastMemberPersistence(CreateDbContext());
    }
    
    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1,2);

    public Domain.Entity.CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMemberType());
    
    public List<Domain.Entity.CastMember> GetExampleCastMembersList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCastMember()).ToList();
}