using DomainEntity = JG.Code.Catalog.Domain.Entity;
using FluentAssertions;
using JG.Code.Catalog.Domain.Exceptions;

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
        var name = _fixture.GetValidName();
        var type = _fixture.GetRandomCastMemberType();
        
        DomainEntity.CastMember castMember = new DomainEntity.CastMember(name, type);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        castMember.Should().NotBeNull();
        castMember.Name.Should().Be(name);
        castMember.Type.Should().Be(type);
        castMember.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (castMember.CreatedAt >= datetimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= datetimeAfter).Should().BeTrue();
    }
    
    [Theory(DisplayName = nameof(ThrowErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ThrowErrorWhenNameIsInvalid(string? name)
    {
        var type = _fixture.GetRandomCastMemberType();
        
        var action = () =>  new DomainEntity.CastMember(name!, type);

        action.Should().Throw<EntityValidationException>().WithMessage($"Name should not be empty or null");
    }
    
    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Update()
    {
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        
        DomainEntity.CastMember castMember = _fixture.GetExampleCastMember();
        castMember.Update(newName, newType);
        
        castMember.Should().NotBeNull();
        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
    }
}