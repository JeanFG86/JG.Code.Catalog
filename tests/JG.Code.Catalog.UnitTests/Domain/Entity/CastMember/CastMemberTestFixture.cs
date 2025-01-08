using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.UnitTests.Common;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.CastMember;

[CollectionDefinition(nameof(CastMemberTestFixture))]
public class CastMemberTestFixtureCollection : ICollectionFixture<CastMemberTestFixture>{}

public class CastMemberTestFixture : BaseFixture
{
    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1,2);
}