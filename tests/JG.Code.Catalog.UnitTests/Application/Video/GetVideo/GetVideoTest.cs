using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Extensions;
using JG.Code.Catalog.Domain.Repository;
using Moq;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.GetVideo;

namespace JG.Code.Catalog.UnitTests.Application.Video.GetVideo;

[Collection(nameof(GetVideoTestFixture))]
public class GetVideoTest
{
    private readonly GetVideoTestFixture _fixture;
    
    public GetVideoTest(GetVideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetVideo))]
    [Trait("Application", "GetVideo - Use Cases")]
    public async Task GetVideo()
    {
        var repositoryMock = new Mock<IVideoRepository>();        
        var exampleVideo = _fixture.GetValidVideo();
        repositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);
        var useCase = new UseCase.GetVideo(repositoryMock.Object);
        var input = new UseCase.GetVideoInput(exampleVideo.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Published.Should().Be(exampleVideo.Published);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        repositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenNotFound))]
    [Trait("Application", "GetVideo - Use Cases")]
    public async Task ThrowsExceptionWhenNotFound()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException("Video not found"));
        var useCase = new UseCase.GetVideo(repositoryMock.Object);
        var input = new UseCase.GetVideoInput(Guid.NewGuid());

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video not found");
        repositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(GetVideoWithAllProperties))]
    [Trait("Application", "GetVideo - Use Cases")]
    public async Task GetVideoWithAllProperties()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        repositoryMock.Setup(x => x.Get(exampleVideo.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);
        var useCase = new UseCase.GetVideo(repositoryMock.Object);
        var input = new UseCase.GetVideoInput(exampleVideo.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.CreatedAt.Should().Be(exampleVideo.CreatedAt);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Published.Should().Be(exampleVideo.Published);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
        output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
        output.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);
        output.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
        output.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
        output.CategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
        output.CastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
        output.GenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
        repositoryMock.VerifyAll();
    }
}