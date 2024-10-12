using FluentAssertions;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using Moq;

namespace JG.Code.Catalog.UnitTests.Application.Genre.ListGenres;

[Collection(nameof(ListGenresTestFixture))]
public class ListGenresTest
{
    private readonly ListGenresTestFixture _fixture;

    public ListGenresTest(ListGenresTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("Application", "ListGenres - Use Cases")]
    public async Task ListGenres()
    {
        var genresExampleList = _fixture.GetExampleGenresList();
        var repositoryMock = _fixture.GetGenreRepositoryMock();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: genresExampleList,
                total: new Random().Next(50, 200)
                );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);
        var useCase = new UseCase.ListGenres(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<GenreModelOutput>)output.Items).ForEach(outputItem =>
        {
            var repositoryGenre = outputRepositorySearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryGenre!.Name);
            outputItem.IsActive.Should().Be(repositoryGenre.IsActive);
            outputItem.CreatedAt.Should().Be(repositoryGenre.CreatedAt);
        });
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ListOkWhenEmpty))]
    [Trait("Application", "ListGenres - Use Cases")]
    public async Task ListOkWhenEmpty()
    {
        var repositoryMock = _fixture.GetGenreRepositoryMock();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<DomainEntity.Genre>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: new List<DomainEntity.Genre>().AsReadOnly(),
                total: 0
                );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>())).ReturnsAsync(outputRepositorySearch);
        var useCase = new UseCase.ListGenres(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
