using FluentAssertions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.DeleteVideo;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using Moq;
using JG.Code.Catalog.Application.Exceptions;

namespace JG.Code.Catalog.UnitTests.Application.Video.DeleteVideo;

[Collection(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTest
{
    private readonly DeleteVideoTestFixture _fixture;
    private readonly UseCase.DeleteVideo _useCase;
    private readonly Mock<IVideoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageService;

    public DeleteVideoTest(DeleteVideoTestFixture fixture)
    {
        _fixture = fixture;
        _repositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storageService = new Mock<IStorageService>();
        _useCase = new UseCase.DeleteVideo(_repositoryMock.Object, _unitOfWorkMock.Object, _storageService.Object);
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

    [Fact(DisplayName = nameof(DeleteVideoWithAllMediasAndClearStorage))]
    [Trait("Application ", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithAllMediasAndClearStorage()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateMedia(_fixture.GetValidMediaPath());
        videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
        var filePaths = new List<string>() { videoExample.Media!.FilePath, videoExample.Trailer!.FilePath };
        var validInput = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == videoExample.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoExample);

        await _useCase.Handle(validInput, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(It.Is<DomainEntity.Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageService.Verify(x => x.Delete(It.Is<string>(filePath => filePaths.Contains(filePath)), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _storageService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyTrailerAndClearStorageOnlyForTrailer))]
    [Trait("Application ", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithOnlyTrailerAndClearStorageOnlyForTrailer()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
        var validInput = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == videoExample.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoExample);

        await _useCase.Handle(validInput, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(It.Is<DomainEntity.Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageService.Verify(x => x.Delete(It.Is<string>(filePath => filePath == videoExample.Trailer!.FilePath), It.IsAny<CancellationToken>()), Times.Once);
        _storageService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyMediaAndClearStorageOnlyForMedia))]
    [Trait("Application ", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithOnlyMediaAndClearStorageOnlyForMedia()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateMedia(_fixture.GetValidMediaPath());
        var validInput = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == videoExample.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoExample);

        await _useCase.Handle(validInput, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(It.Is<DomainEntity.Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageService.Verify(x => x.Delete(It.Is<string>(filePath => filePath == videoExample.Media!.FilePath), It.IsAny<CancellationToken>()), Times.Once);
        _storageService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(DeleteVideoWithoutAnyMediaAndDontClearStorage))]
    [Trait("Application ", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithoutAnyMediaAndDontClearStorage()
    {
        var videoExample = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(videoExample.Id);
        _repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == videoExample.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoExample);

        await _useCase.Handle(validInput, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(It.Is<DomainEntity.Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(ThrowsNotFoundExceptionWhenVideoNotFound))]
    [Trait("Application ", "DeleteVideo - Use Cases")]
    public async Task ThrowsNotFoundExceptionWhenVideoNotFound()
    {
        var input = _fixture.GetValidInput();
        _repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Vídeo not found"));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Vídeo not found");
        _repositoryMock.VerifyAll();
        _repositoryMock.Verify(x => x.Delete(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _storageService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
