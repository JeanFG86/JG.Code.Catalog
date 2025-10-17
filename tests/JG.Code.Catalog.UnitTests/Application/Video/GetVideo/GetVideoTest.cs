using FluentAssertions;
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
        output.Rating.Should().Be(exampleVideo.Rating);
        repositoryMock.VerifyAll();
    }
}