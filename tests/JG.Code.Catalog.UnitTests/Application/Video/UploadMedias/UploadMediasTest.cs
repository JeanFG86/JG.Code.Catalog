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
        _repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_fixture.GetValidVideo());
        _storageService.Setup(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());

        await _useCase.Handle(input: _fixture.GetValidInput(), cancellationToken: CancellationToken.None);
        
        _repositoryMock.VerifyAll();
        _storageService.Verify(x => x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}