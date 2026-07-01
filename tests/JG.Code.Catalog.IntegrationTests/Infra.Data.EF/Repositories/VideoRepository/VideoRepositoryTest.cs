using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
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
        await dbContextAct.SaveChangesAsync();

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
    
    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Delete()
    {
        CodeCatalogDbContext dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var dbContextAct = _fixture.CreateDbContext(true);
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContextAct);

        await videoRepository.Delete(exampleVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteWithRelations))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task DeleteWithRelations()
    {
        CodeCatalogDbContext dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        var categories = _fixture.GetRandomCategoryList();
        var genres = _fixture.GetRandomGenreList();
        var castMembers = _fixture.GetRandomCastMemberList();

        dbContextArrange.AddRange(categories);
        dbContextArrange.AddRange(genres);
        dbContextArrange.AddRange(castMembers);

        categories.ForEach(c => exampleVideo.AddCategory(c.Id));
        genres.ForEach(g => exampleVideo.AddGenre(g.Id));
        castMembers.ForEach(cm => exampleVideo.AddCastMember(cm.Id));

        IVideoRepository arrangeRepository = new Repository.VideoRepository(dbContextArrange);
        await arrangeRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContextArrange.SaveChangesAsync();

        var dbContextAct = _fixture.CreateDbContext(true);
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContextAct);

        await videoRepository.Delete(exampleVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().BeNull();

        var dbCategories = await assertsDbContext.Set<VideosCategories>()
            .Where(r => r.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbCategories.Should().BeEmpty();

        var dbGenres = await assertsDbContext.Set<VideosGenres>()
            .Where(r => r.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbGenres.Should().BeEmpty();

        var dbCastMembers = await assertsDbContext.Set<VideosCastMembers>()
            .Where(r => r.VideoId == exampleVideo.Id)
            .ToListAsync();
        dbCastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(UpdateEntitiesAndValueObjects))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task UpdateEntitiesAndValueObjects()
    {
        CodeCatalogDbContext dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var dbContextAct = _fixture.CreateDbContext(true);
        var updateThumb = _fixture.GetValidImagePath();
        var updateThumbHalf = _fixture.GetValidImagePath();
        var updateBanner = _fixture.GetValidImagePath();
        var updateMedia = _fixture.GetValidMediaPath();
        var updateTrailer = _fixture.GetValidMediaPath();
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContextAct);

        exampleVideo.UpdateThumb(updateThumb);
        exampleVideo.UpdateThumbHalf(updateThumbHalf);
        exampleVideo.UpdateBanner(updateBanner);
        exampleVideo.UpdateMedia(updateMedia);
        exampleVideo.UpdateTrailer(updateTrailer);
        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);

        dbVideo.Should().NotBeNull();
        dbVideo!.Thumb.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(updateThumb);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf!.Path.Should().Be(updateThumbHalf);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner!.Path.Should().Be(updateBanner);
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Media!.FilePath.Should().Be(updateMedia);
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Trailer!.FilePath.Should().Be(updateTrailer);
    }

    [Fact(DisplayName = nameof(UpdateWithRelations))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task UpdateWithRelations()
    {
        var arrangeContext = _fixture.CreateDbContext();
        var video = _fixture.GetValidVideo();
        var oldCategory = _fixture.GetExampleCategory();
        var oldGenre = _fixture.GetExampleGenre();
        var oldCastMember = _fixture.GetExampleCastMember();
        arrangeContext.AddRange(oldCategory, oldGenre, oldCastMember);
        video.AddCategory(oldCategory.Id);
        video.AddGenre(oldGenre.Id);
        video.AddCastMember(oldCastMember.Id);
        await new Repository.VideoRepository(arrangeContext).Insert(video, CancellationToken.None);
        await arrangeContext.SaveChangesAsync();

        var categories = _fixture.GetRandomCategoryList();
        var genres = _fixture.GetRandomGenreList();
        var castMembers = _fixture.GetRandomCastMemberList();
        var actContext = _fixture.CreateDbContext(true);
        actContext.AddRange(categories);
        actContext.AddRange(genres);
        actContext.AddRange(castMembers);
        video.RemoveAllCategories();
        video.RemoveAllGenres();
        video.RemoveAllCastMembers();
        categories.ForEach(category => video.AddCategory(category.Id));
        genres.ForEach(genre => video.AddGenre(genre.Id));
        castMembers.ForEach(castMember => video.AddCastMember(castMember.Id));

        await new Repository.VideoRepository(actContext).Update(video, CancellationToken.None);
        await actContext.SaveChangesAsync();

        var assertContext = _fixture.CreateDbContext(true);
        (await assertContext.VideosCategories.Where(x => x.VideoId == video.Id).Select(x => x.CategoryId).ToListAsync())
            .Should().BeEquivalentTo(categories.Select(x => x.Id));
        (await assertContext.VideosGenres.Where(x => x.VideoId == video.Id).Select(x => x.GenreId).ToListAsync())
            .Should().BeEquivalentTo(genres.Select(x => x.Id));
        (await assertContext.VideosCastMembers.Where(x => x.VideoId == video.Id).Select(x => x.CastMemberId).ToListAsync())
            .Should().BeEquivalentTo(castMembers.Select(x => x.Id));
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Get()
    {
        var arrangeContext = _fixture.CreateDbContext();
        var video = _fixture.GetValidVideo();
        video.UpdateThumb(_fixture.GetValidImagePath());
        video.UpdateMedia(_fixture.GetValidMediaPath());
        await arrangeContext.AddAsync(video);
        await arrangeContext.SaveChangesAsync();

        var result = await new Repository.VideoRepository(_fixture.CreateDbContext(true))
            .Get(video.Id, CancellationToken.None);

        result.Id.Should().Be(video.Id);
        result.Title.Should().Be(video.Title);
        result.Thumb.Should().Be(video.Thumb);
        result.Media.Should().NotBeNull();
        result.Media!.FilePath.Should().Be(video.Media!.FilePath);
        result.Media.Status.Should().Be(video.Media.Status);
    }

    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task GetThrowsWhenNotFound()
    {
        var id = Guid.NewGuid();
        var repository = new Repository.VideoRepository(_fixture.CreateDbContext());

        var action = () => repository.Get(id, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Video '{id}' not found.");
    }

    [Fact(DisplayName = nameof(GetWithRelations))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task GetWithRelations()
    {
        var arrangeContext = _fixture.CreateDbContext();
        var video = _fixture.GetValidVideo();
        var categories = _fixture.GetRandomCategoryList();
        var genres = _fixture.GetRandomGenreList();
        var castMembers = _fixture.GetRandomCastMemberList();
        arrangeContext.AddRange(categories);
        arrangeContext.AddRange(genres);
        arrangeContext.AddRange(castMembers);
        categories.ForEach(x => video.AddCategory(x.Id));
        genres.ForEach(x => video.AddGenre(x.Id));
        castMembers.ForEach(x => video.AddCastMember(x.Id));
        await new Repository.VideoRepository(arrangeContext).Insert(video, CancellationToken.None);
        await arrangeContext.SaveChangesAsync();

        var result = await new Repository.VideoRepository(_fixture.CreateDbContext(true))
            .Get(video.Id, CancellationToken.None);

        result.Categories.Should().BeEquivalentTo(categories.Select(x => x.Id));
        result.Genres.Should().BeEquivalentTo(genres.Select(x => x.Id));
        result.CastMembers.Should().BeEquivalentTo(castMembers.Select(x => x.Id));
    }

    [Fact(DisplayName = nameof(Search))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Search()
    {
        var arrangeContext = _fixture.CreateDbContext();
        var videos = Enumerable.Range(0, 15).Select(_ => _fixture.GetValidVideo()).ToList();
        await arrangeContext.AddRangeAsync(videos);
        await arrangeContext.SaveChangesAsync();
        var input = new SearchInput(2, 5, "", "title", SearchOrder.Asc);

        var result = await new Repository.VideoRepository(_fixture.CreateDbContext(true))
            .Search(input, CancellationToken.None);

        result.CurrentPage.Should().Be(2);
        result.PerPage.Should().Be(5);
        result.Total.Should().Be(15);
        result.Items.Should().HaveCount(5).And.BeInAscendingOrder(x => x.Title);
    }

    [Fact(DisplayName = nameof(SearchByTitle))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task SearchByTitle()
    {
        var arrangeContext = _fixture.CreateDbContext();
        var videos = new[]
        {
            _fixture.GetValidVideo("A Course Video"),
            _fixture.GetValidVideo("Another Course Video"),
            _fixture.GetValidVideo("Unrelated")
        };
        await arrangeContext.AddRangeAsync(videos);
        await arrangeContext.SaveChangesAsync();
        var input = new SearchInput(1, 10, "Course", "title", SearchOrder.Desc);

        var result = await new Repository.VideoRepository(_fixture.CreateDbContext(true))
            .Search(input, CancellationToken.None);

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2).And.BeInDescendingOrder(x => x.Title);
        result.Items.Should().OnlyContain(x => x.Title.Contains("Course"));
    }

    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task SearchReturnsRelations()
    {
        var arrangeContext = _fixture.CreateDbContext();
        var video = _fixture.GetValidVideo();
        var category = _fixture.GetExampleCategory();
        var genre = _fixture.GetExampleGenre();
        var castMember = _fixture.GetExampleCastMember();
        arrangeContext.AddRange(category, genre, castMember);
        video.AddCategory(category.Id);
        video.AddGenre(genre.Id);
        video.AddCastMember(castMember.Id);
        await new Repository.VideoRepository(arrangeContext).Insert(video, CancellationToken.None);
        await arrangeContext.SaveChangesAsync();
        var input = new SearchInput(1, 10, "", "title", SearchOrder.Asc);

        var result = await new Repository.VideoRepository(_fixture.CreateDbContext(true))
            .Search(input, CancellationToken.None);

        var returnedVideo = result.Items.Should().ContainSingle().Subject;
        returnedVideo.Categories.Should().ContainSingle().Which.Should().Be(category.Id);
        returnedVideo.Genres.Should().ContainSingle().Which.Should().Be(genre.Id);
        returnedVideo.CastMembers.Should().ContainSingle().Which.Should().Be(castMember.Id);
    }
}
