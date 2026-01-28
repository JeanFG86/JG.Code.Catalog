using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Extensions;
using JG.Code.Catalog.Domain.Repository;
using Moq;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.UpdateVideo;

namespace JG.Code.Catalog.UnitTests.Application.Video.UpdateVideo;

[Collection(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideoTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepositoryMock = new();
    private readonly Mock<IGenreRepository> _genreRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UseCase.UpdateVideo _useCase;

    public UpdateVideoTest(UpdateVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new UseCase.UpdateVideo(_videoRepositoryMock.Object, _genreRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact(DisplayName = nameof(UpdateVideosBasicInfo))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosBasicInfo()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id);
        _videoRepositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepositoryMock.VerifyAll();
        _videoRepositoryMock.Verify(x => x.Update(It.Is<DomainEntity.Video>(video =>
            video.Title == input.Title &&
            video.Description == input.Description &&
            video.YearLaunched == input.YearLaunched &&
            video.Duration == input.Duration &&
            video.Opened == input.Opened &&
            video.Published == input.Published &&
            video.Rating == input.Rating
        ), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Description.Should().Be(input.Description);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Published.Should().Be(input.Published);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenVideoNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenVideoNotFound()
    {
        var input = _fixture.CreateValidInput(Guid.NewGuid());
        _videoRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException("Video not found"));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video not found");
        _videoRepositoryMock.Verify(x => x.Update(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}
