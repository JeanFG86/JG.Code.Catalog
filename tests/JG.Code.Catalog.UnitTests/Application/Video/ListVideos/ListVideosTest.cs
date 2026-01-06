using JG.Code.Catalog.Domain.Repository;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.ListVideos;
using Moq;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.Application.UseCases.Video.ListVideos;
using FluentAssertions;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Extensions;

namespace JG.Code.Catalog.UnitTests.Application.Video.ListVideos;

[Collection(nameof(ListVideosTestFixtureCollection))]
public class ListVideosTest
{
    private readonly ListVideosTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly UseCase.ListVideos _useCase;

    public ListVideosTest(ListVideosTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _genreRepositoryMock = new Mock<IGenreRepository>();
        _useCase = new UseCase.ListVideos(_videoRepositoryMock.Object, _categoryRepositoryMock.Object, _genreRepositoryMock.Object);
    }

    [Fact(DisplayName =nameof(ListVideos))]
    [Trait("Application","ListVideos - Use Cases")]
    public async Task ListVideos ()
    {
        var exampleVideosList = _fixture.CreateExampleVideosList();
        var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        _videoRepositoryMock.Setup(x => x.Search(It.Is<SearchInput>(x => 
                                    x.Page == input.Page &&
                                    x.PerPage == input.PerPage &&
                                    x.Search == input.Search &&
                                    x.OrderBy == input.Sort &&
                                    x.Order == input.Dir), It.IsAny<CancellationToken>())).ReturnsAsync(new SearchOutput<DomainEntity.Video>(input.Page, input.PerPage, exampleVideosList.Count, exampleVideosList));

        PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleVideo = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleVideo.Should().NotBeNull();
            output.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleVideo!.Id);
            outputItem.Title.Should().Be(exampleVideo!.Title);
            outputItem.Description.Should().Be(exampleVideo.Description);
            outputItem.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            outputItem.Opened.Should().Be(exampleVideo.Opened);
            outputItem.Published.Should().Be(exampleVideo.Published);
            outputItem.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
            outputItem.Duration.Should().Be(exampleVideo.Duration);
            outputItem.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
            outputItem.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
            outputItem.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
            outputItem.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);
            outputItem.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
            outputItem.CategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
            outputItem.GenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
            outputItem.CastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
        });
    }

    [Fact(DisplayName = nameof(ListVideosWithRelations))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task ListVideosWithRelations()
    {
        var (exampleVideosList, exampleCaregoriesList, exampleGenresList) = _fixture.CreateExampleVideosListWithRelations();
        var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var examplesCategoriesIds = exampleCaregoriesList.Select(cat => cat.Id).ToList();
        var exampleGenresIds = exampleGenresList.Select(genre => genre.Id).ToList();
        _categoryRepositoryMock.Setup(x => x.GetListByIds(It.Is<List<Guid>>(list => list.All(examplesCategoriesIds.Contains) && list.Count == examplesCategoriesIds.Count), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCaregoriesList);
        _genreRepositoryMock.Setup(x => x.GetListByIds(It.Is<List<Guid>>(list => list.All(exampleGenresIds.Contains) && list.Count == exampleGenresIds.Count), It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenresList);
        _videoRepositoryMock.Setup(x => x.Search(It.Is<SearchInput>(x =>
                                    x.Page == input.Page &&
                                    x.PerPage == input.PerPage &&
                                    x.Search == input.Search &&
                                    x.OrderBy == input.Sort &&
                                    x.Order == input.Dir), It.IsAny<CancellationToken>())).ReturnsAsync(new SearchOutput<DomainEntity.Video>(input.Page, input.PerPage, exampleVideosList.Count, exampleVideosList));

        PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleVideo = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleVideo.Should().NotBeNull();
            output.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleVideo!.Id);
            outputItem.Title.Should().Be(exampleVideo!.Title);
            outputItem.Description.Should().Be(exampleVideo.Description);
            outputItem.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            outputItem.Opened.Should().Be(exampleVideo.Opened);
            outputItem.Published.Should().Be(exampleVideo.Published);
            outputItem.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
            outputItem.Duration.Should().Be(exampleVideo.Duration);
            outputItem.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
            outputItem.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
            outputItem.TrailerFileUrl.Should().Be(exampleVideo.Trailer!.FilePath);
            outputItem.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);
            outputItem.VideoFileUrl.Should().Be(exampleVideo.Media!.FilePath);
            outputItem.CategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
            outputItem.GenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
            outputItem.CastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
            outputItem.Categories.ToList().ForEach(outputItemCategory =>
            {
                var exampleCategory = exampleCaregoriesList.Find(x => x.Id == outputItemCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputItemCategory.Name.Should().Be(exampleCategory?.Name);
            });
            outputItem.Genres.ToList().ForEach(outputItemGenre =>
            {
                var exampleGenre = exampleGenresList.Find(x => x.Id == outputItemGenre.Id);
                exampleGenre.Should().NotBeNull();
                outputItemGenre.Name.Should().Be(exampleGenre?.Name);
            });
            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();
            _videoRepositoryMock.VerifyAll();
        });
    }

    [Fact(DisplayName = nameof(ListVideosWithRelations))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task ListVideosWithoutRelationsDoesentCallOtherRepositories()
    {
        var exampleVidedos = _fixture.CreateExampleVideosListWithoutRelations();
        var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);      
        _videoRepositoryMock.Setup(x => x.Search(It.Is<SearchInput>(x =>
                                    x.Page == input.Page &&
                                    x.PerPage == input.PerPage &&
                                    x.Search == input.Search &&
                                    x.OrderBy == input.Sort &&
                                    x.Order == input.Dir), It.IsAny<CancellationToken>())).ReturnsAsync(new SearchOutput<DomainEntity.Video>(input.Page, input.PerPage, exampleVidedos.Count, exampleVidedos));

        PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVidedos.Count);
        output.Items.Should().HaveCount(exampleVidedos.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleVideo = exampleVidedos.Find(x => x.Id == outputItem.Id);
            exampleVideo.Should().NotBeNull();
            output.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleVideo!.Id);
            outputItem.Title.Should().Be(exampleVideo!.Title);
            outputItem.CastMembers.Should().HaveCount(0);
            outputItem.Categories.Should().HaveCount(0);
            outputItem.Genres.Should().HaveCount(0);            
            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.Verify(x => x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
            _genreRepositoryMock.Verify(x => x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        });
    }

    [Fact(DisplayName = nameof(ListReturnsEmptyWhenThereIsNoVideos))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task ListReturnsEmptyWhenThereIsNoVideos()
    {
        var emptyVideosList = new List<DomainEntity.Video>();
        var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);

        _videoRepositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(x =>
                x.Page == input.Page &&
                x.PerPage == input.PerPage &&
                x.Search == input.Search &&
                x.OrderBy == input.Sort &&
                x.Order == input.Dir),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchOutput<DomainEntity.Video>(
                input.Page,
                input.PerPage,
                0,
                emptyVideosList));

        PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().BeEmpty();
    }
}
