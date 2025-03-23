using FluentAssertions;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMembers;



[Collection(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTest
{
    private readonly ListCastMembersTestFixture _fixture;

    public ListCastMembersTest(ListCastMembersTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(ListCastMembers))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    public async Task ListCastMembers()
    {
        List<DomainEntity.CastMember> exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleCastMembers);
        await arrangeDbContext.SaveChangesAsync();
        var useCase = new UseCase.ListCastMembers(new CastMemberRepository(_fixture.CreateDbContext(true)));
        var input = new UseCase.ListCastMembersInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCastMembers.Count);
        output.Items.Should().HaveCount(exampleCastMembers.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleCastMembers.Find(example => example.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
        });
    }
    
    [Fact(DisplayName = nameof(ListCastMembersReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    public async Task ListCastMembersReturnsEmptyWhenPersistenceIsEmpty()
    {
        UseCase.ListCastMembers useCase = new UseCase.ListCastMembers(new CastMemberRepository(_fixture.CreateDbContext()));
        var input = new UseCase.ListCastMembersInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }
    
    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
        int quantityCastMembersToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersListList = _fixture.GetExampleCastMembersList(quantityCastMembersToGenerate);
        await dbContext.AddRangeAsync(exampleCastMembersListList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var input = new UseCase.ListCastMembersInput(page, perPage);
        var useCase = new UseCase.ListCastMembers(castMemberRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCastMembersListList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        foreach (var outputIem in output.Items)
        {
            var exampleItem = exampleCastMembersListList.Find(c => c.Id == outputIem.Id);
            exampleItem.Should().NotBeNull();
            outputIem!.Name.Should().Be(exampleItem!.Name);
            outputIem.Type.Should().Be(exampleItem.Type);
            outputIem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
    
     [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Robots", 1, 5, 2, 2)]
    [InlineData("Comedy", 2, 3, 0, 0)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var castMembersNamesList = new List<string>{
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based onReal Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        };
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersListWithNames(castMembersNamesList);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var input = new UseCase.ListCastMembersInput(page, perPage, search);
        var useCase = new UseCase.ListCastMembers(castMemberRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (var outputIem in output.Items)
        {
            var exampleItem = exampleCastMembersList.Find(c => c.Id == outputIem.Id);
            exampleItem.Should().NotBeNull();
            outputIem!.Name.Should().Be(exampleItem!.Name);
            outputIem.Type.Should().Be(exampleItem.Type);
            outputIem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
    
    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdat", "asc")]
    [InlineData("createdat", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new UseCase.ListCastMembersInput(1, 20, "", orderBy, searchOrder);
        var useCase = new UseCase.ListCastMembers(castMemberRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneCastMembersListOrdered(exampleCastMembersList, input.Sort, input.Dir);
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCastMembersList.Count);
        output.Items.Should().HaveCount(exampleCastMembersList.Count);
        for (var indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var outputIem = output.Items[indice];
            var exampleItem = expectedOrderedList[indice];
            exampleItem.Should().NotBeNull();
            outputIem.Id.Should().Be(exampleItem!.Id);
            outputIem!.Name.Should().Be(exampleItem!.Name);
            outputIem.Type.Should().Be(exampleItem.Type);
            outputIem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
}