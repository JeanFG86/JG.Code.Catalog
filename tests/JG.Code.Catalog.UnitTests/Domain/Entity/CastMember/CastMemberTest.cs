using DomainEntity = JG.Code.Catalog.Domain.Entity;
using FluentAssertions;
using JG.Code.Catalog.Domain.Enum;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.CastMember;

[Collection(nameof(CastMemberTestFixture))]
public class CastMemberTest
{
    private readonly CastMemberTestFixture _fixture;

    public CastMemberTest(CastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Instantiate()
    {
        var datetimeBefore = DateTime.Now;
        var name = "Jorge Lucas";
        var type = CastMemberType.Director;
        
        DomainEntity.CastMember castMember = new DomainEntity.CastMember(name, type);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        castMember.Should().NotBeNull();
        castMember.Name.Should().Be(name);
        castMember.Type.Should().Be(type);
        castMember.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (castMember.CreatedAt >= datetimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= datetimeAfter).Should().BeTrue();
    }
}