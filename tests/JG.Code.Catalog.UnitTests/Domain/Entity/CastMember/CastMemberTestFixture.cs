using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.UnitTests.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.CastMember;

[CollectionDefinition(nameof(CastMemberTestFixture))]
public class CastMemberTestFixtureCollection : ICollectionFixture<CastMemberTestFixture>{}

public class CastMemberTestFixture : BaseFixture
{
    public DomainEntity.CastMember GetExampleCastMember() => new DomainEntity.CastMember(GetValidName(), GetRandomCastMemberType());
    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1,2);
}