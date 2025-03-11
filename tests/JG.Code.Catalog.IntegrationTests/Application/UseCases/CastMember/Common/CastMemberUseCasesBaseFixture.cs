using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.IntegrationTests.Common;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;

public class CastMemberUseCasesBaseFixture : BaseFixture
{
    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1,2);

    public Domain.Entity.CastMember GetExampleCastMember()
        => new(GetValidName(), GetRandomCastMemberType());
}