using FluentAssertions;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
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
    
    [Fact(DisplayName = nameof(InsertWithRelations))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task InsertWithRelations()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        var categories = _fixture.GetRandomCategoryList();
        var genres = _fixture.GetRandomGenreList();
        var castMembers = _fixture.GetRandomCastMemberList();

        dbContext.AddRange(categories);
        dbContext.AddRange(genres);
        dbContext.AddRange(castMembers);

        categories.ForEach(c => exampleVideo.AddCategory(c.Id));
        genres.ForEach(g => exampleVideo.AddGenre(g.Id));
        castMembers.ForEach(cm => exampleVideo.AddCastMember(cm.Id));

        IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);
        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();

        var dbCategories = await assertsDbContext.Set<VideosCategories>()
            .Where(r => r.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbCategories.Should().HaveCount(categories.Count);
        dbCategories.Select(r => r.CategoryId).Should().BeEquivalentTo(categories.Select(c => c.Id));

        var dbGenres = await assertsDbContext.Set<VideosGenres>()
            .Where(r => r.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbGenres.Should().HaveCount(genres.Count);
        dbGenres.Select(r => r.GenreId).Should().BeEquivalentTo(genres.Select(g => g.Id));

        var dbCastMembers = await assertsDbContext.Set<VideosCastMembers>()
            .Where(r => r.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbCastMembers.Should().HaveCount(castMembers.Count);
        dbCastMembers.Select(r => r.CastMemberId).Should().BeEquivalentTo(castMembers.Select(cm => cm.Id));
    }
    
    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Update()
    {
        CodeCatalogDbContext dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var newValues = _fixture.GetValidVideo();
        var dbContextAct = _fixture.CreateDbContext(true);
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContextAct);

        exampleVideo.Update(newValues.Title, newValues.Description, newValues.YearLaunched, newValues.Opened, newValues.Published, newValues.Duration, newValues.Rating);
        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await dbContextArrange.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.Rating.Should().Be(exampleVideo.Rating);
        dbVideo.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }
}
