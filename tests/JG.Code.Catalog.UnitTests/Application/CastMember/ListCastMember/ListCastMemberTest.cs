using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using Moq;
using UseCase = JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Application.CastMember.ListCastMember;

[Collection(nameof(ListCastMemberTestFixture))]
public class ListCastMemberTest
{
    private readonly ListCastMemberTestFixture _fixture;

    public ListCastMemberTest(ListCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCastMembers - Use Cases")]
    public async Task List()
    {
        var categoriesExampleList = _fixture.GetExampleCastMemberList(3);
        var repositoryMock = new Mock<ICastMemberRepository>();
        var input = _fixture.GetExampleInput();
        var outputRepositorySearch = new SearchOutput<DomainEntity.CastMember>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: categoriesExampleList,
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
        var useCase = new UseCase.ListCastMembers(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<CastMemberModelOutput>)output.Items).ForEach(outputItem =>
        {
            var repositoryCategory = outputRepositorySearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryCategory!.Name);
            outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
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
}