using FluentAssertions;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoTest
{
    private VideoTestFixture _fixture;

    public VideoTest(VideoTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Video - Aggregates")]
    public void Instantiate()
    {
        var video = new DomainEntity.Video("Title", "Description", true, true, 2001, 180);

        video.Should().NotBeNull();
        video.Title.Should().Be("Title");
        video.Description.Should().Be("Description");
        video.Opened.Should().Be(true);
        video.Published.Should().Be(true);
        video.YearLaunched.Should().Be(2001);
        video.Duration.Should().Be(180);

    }
}