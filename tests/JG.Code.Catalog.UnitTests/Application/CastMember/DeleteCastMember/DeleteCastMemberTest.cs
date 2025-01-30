using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;
using UsesCases = JG.Code.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.DeleteCastMember;

[Collection(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTest
{
    private readonly DeleteCastMemberTestFixture _fixture;

    public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(DeleteCastMember))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async Task DeleteCastMember()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = _fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(castMemberExample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(castMemberExample);
        var input = new UsesCases.DeleteCastMemberInput(castMemberExample.Id);
        var useCase = new UsesCases.DeleteCastMember(repositoryMock.Object, unitOfWorkMock.Object);

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(castMemberExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Delete(castMemberExample, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}