using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Enum;
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
}