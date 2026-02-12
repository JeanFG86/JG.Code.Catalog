using FluentAssertions;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Infra.Data.EF;
using Repository = JG.Code.Catalog.Infra.Data.EF.Repositories;

namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;


[Collection(nameof(VideoRepositoryTestFixture))] 
public class VideoRepositoryTest
{
    private readonly VideoRepositoryTestFixture _fixture;
    public VideoRepositoryTest(VideoRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Insert()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo!.Title.Should().Be(exampleVideo.Title);

        dbVideo.Thumb.Should().Be(exampleVideo.Thumb);
        dbVideo.ThumbHalf.Should().Be(exampleVideo.ThumbHalf);
        dbVideo.Banner.Should().Be(exampleVideo.Banner);
        dbVideo.Media.Should().Be(exampleVideo.Media);
        dbVideo.Trailer.Should().Be(exampleVideo.Trailer);

        dbVideo.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();

    }
}
