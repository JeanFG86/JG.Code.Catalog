using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using UseCase = JG.Code.Catalog.Application.UseCases.CastMember.GetCastMember;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.Repository;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.GetCastMember;

[Collection(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTest
{
    private readonly GetCastMemberTestFixture _fixture;

    public GetCastMemberTest(GetCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCastMember))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task GetCastMember()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var castMemberExample = _fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(castMemberExample);
        var input = new UseCase.GetCastMemberInput(castMemberExample.Id);
        var useCase = new UseCase.GetCastMember(repositoryMock.Object);
        
        CastMemberModelOutput output = await useCase.Handle(input, CancellationToken.None);
        
        repositoryMock.Verify(repository => repository.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(castMemberExample.Name);
        output.Type.Should().Be(castMemberExample.Type);
        output.Id.Should().Be(castMemberExample.Id);
        output.CreatedAt.Should().Be(castMemberExample.CreatedAt);
    }
    
    [Fact(DisplayName = nameof(ThrowIfNotFound))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task ThrowIfNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException("notfound"));
        var input = new UseCase.GetCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.GetCastMember(repositoryMock.Object);
        
        var action = async () =>await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }
}