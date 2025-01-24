using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.UnitTests.Common;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.Common;

public class CastMemberUseCasesBaseFixture : BaseFixture
{
    public Catalog.Domain.Entity.CastMember GetExampleCastMember() => new(GetValidName(), GetRandomCastMemberType());
    
    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1,2);
}