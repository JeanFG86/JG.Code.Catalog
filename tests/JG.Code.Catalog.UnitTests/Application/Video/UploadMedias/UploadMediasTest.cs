using FluentAssertions;
using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;
using Moq;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.UploadMideas;

namespace JG.Code.Catalog.UnitTests.Application.Video.UploadMideas;

[Collection(nameof(UploadMideasTestFixture))]
public class UploadMediasTest
{
    private readonly UploadMideasTestFixture _fixture;
    private readonly UseCase.UploadMedias _useCase;
    private readonly Mock<IVideoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageService;
    
    public UploadMediasTest(UploadMideasTestFixture fixture)
    {
        _fixture = fixture;
        _repositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storageService = new Mock<IStorageService>();
        _useCase = new UseCase.UploadMedias(_repositoryMock.Object, _storageService.Object, _unitOfWorkMock.Object);
    }
    
    [Fact(DisplayName = nameof(UploadMedias))]
    [Trait("Application ", "UploadMedias - Use Cases")]
    public async void UploadMedias()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(video.Id);
        var fileNames = new List<string> { StorageFileName.Create(video.Id, nameof(video.Media), validInput.VideoFile!.Extension),  StorageFileName.Create(video.Id, nameof(video.Trailer), validInput.TrailerFile!.Extension)};
        _repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _storageService.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());

        await _useCase.Handle(validInput, cancellationToken: CancellationToken.None);
        
        _repositoryMock.VerifyAll();
        _storageService.Verify(x => x.Upload(It.Is<string>(x => fileNames.Contains(x)), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
    [Trait("Application ", "UploadMedias - Use Cases")]
    public async void ThrowsWhenVideoNotFound()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(video.Id);
        _repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video bot found"));

        var action = () => _useCase.Handle(validInput, cancellationToken: CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video bot found");
    }
    
    [Fact(DisplayName = nameof(ClearStorageInUploadErrorCase))]
    [Trait("Application ", "UploadMedias - Use Cases")]
    public async void ClearStorageInUploadErrorCase()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(video.Id);
        var videoFileName = StorageFileName.Create(video.Id, nameof(video.Media), validInput.VideoFile!.Extension);
        var videoStoragePath = $"storage/{videoFileName}";
        var trailerFileName = StorageFileName.Create(video.Id, nameof(video.Trailer), validInput.TrailerFile!.Extension);
        var fileNames = new List<string> { videoFileName, trailerFileName};
        _repositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == video.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);
        _storageService.Setup(x => x.Upload(It.Is<string>(x => x == videoFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoStoragePath);
        _storageService.Setup(x => x.Upload(It.Is<string>(x => x == trailerFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong whit the upload"));
        
        var action = () => _useCase.Handle(validInput, cancellationToken: CancellationToken.None);
        
        await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong whit the upload");
        _repositoryMock.VerifyAll();
        _storageService.Verify(x => x.Upload(It.Is<string>(x => fileNames.Contains(x)), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _storageService.Verify(x => x.Delete(It.Is<string>(x => x == videoStoragePath), It.IsAny<CancellationToken>()), Times.Once);
    }
}