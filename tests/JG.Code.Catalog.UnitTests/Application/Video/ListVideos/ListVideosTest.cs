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
    private readonly UseCase.ListVideos _useCase;

    public ListVideosTest(ListVideosTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _useCase = new UseCase.ListVideos(_videoRepositoryMock.Object);
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
