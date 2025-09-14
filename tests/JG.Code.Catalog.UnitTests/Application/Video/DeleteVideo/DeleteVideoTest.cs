using FluentAssertions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.DeleteVideo;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.Video.DeleteVideo;

[Collection(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTest
{
    private readonly DeleteVideoTestFixture _fixture;
    private readonly UseCase.DeleteVideo _useCase;
    private readonly Mock<IVideoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteVideoTest(DeleteVideoTestFixture fixture)
    {
        _fixture = fixture;
        _repositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new UseCase.DeleteVideo(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact(DisplayName = nameof(DeleteVideo))]
    [Trait("Application ", "DeleteVideo - Use Cases")]
    public async Task DeleteVideo()
    {
        var videoExample = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == videoExample.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoExample);

        await _useCase.Handle(validInput, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(It.Is<DomainEntity.Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
