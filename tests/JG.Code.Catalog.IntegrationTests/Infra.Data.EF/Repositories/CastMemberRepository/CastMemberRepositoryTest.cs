using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.Infra.Data.EF;
using Repository = JG.Code.Catalog.Infra.Data.EF.Repositories;

namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;


[Collection(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTest
{
    private readonly CastMemberRepositoryTestFixture _fixture;

    public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Insert()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);

        await castMemberRepository.Insert(exampleCastMember, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCastMember = await (_fixture.CreateDbContext(true)).CastMembers.FindAsync(exampleCastMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }
    
    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Get()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        exampleCastMembersList.Add(exampleCastMember);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));        

        var dbCastMember = await categoryRepository.Get(exampleCastMember.Id, CancellationToken.None);

        dbCastMember.Should().NotBeNull();
        dbCastMember!.Id.Should().Be(exampleCastMember.Id);
        dbCastMember!.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }
    
    [Fact(DisplayName = nameof(GetThrowIfNotFound))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task GetThrowIfNotFound()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        await dbContext.AddRangeAsync(_fixture.GetExampleCastMembersList(15));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);

        var task = async () => await castMemberRepository.Get(exampleId, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"CastMember '{exampleId}' not found.");
    }
    
    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Delete()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        exampleCastMembersList.Add(exampleCastMember);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);
        
        await castMemberRepository.Delete(exampleCastMember, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCastMember = await (_fixture.CreateDbContext(true)).CastMembers.FindAsync(exampleCastMember.Id);
        dbCastMember.Should().BeNull();      
    }
    
    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Update()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var newCastMemberValues = _fixture.GetExampleCastMember();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        exampleCastMembersList.Add(exampleCastMember);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);

        exampleCastMember.Update(newCastMemberValues.Name, newCastMemberValues.Type);
        await castMemberRepository.Update(exampleCastMember, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCastMember = await (_fixture.CreateDbContext(true)).CastMembers.FindAsync(exampleCastMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Id.Should().Be(exampleCastMember.Id);
        dbCastMember!.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }
    
    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task SearchReturnsListAndTotal()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(15);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCastMembersList.Count);
        output.Items.Should().HaveCount(exampleCastMembersList.Count);
        foreach (CastMember outputIem in output.Items)
        {
            var exampleItem = exampleCastMembersList.Find(CastMember => CastMember.Id == outputIem.Id);
            exampleItem.Should().NotBeNull();
            outputIem!.Name.Should().Be(exampleItem!.Name);
            outputIem.Type.Should().Be(exampleItem.Type);
            outputIem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }        
    }
    
    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpry))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpry()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);        
    }
    
    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
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
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMembesList = _fixture.GetExampleCastMembersList(quantityCastMembersToGenerate);
        await dbContext.AddRangeAsync(exampleCastMembesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(quantityCastMembersToGenerate);
        output.Items.Should().HaveCount(expectedQuantityItems);
        foreach (CastMember outputIem in output.Items)
        {
            var exampleItem = exampleCastMembesList.Find(castMember => castMember.Id == outputIem.Id);
            exampleItem.Should().NotBeNull();
            outputIem!.Name.Should().Be(exampleItem!.Name);
            outputIem.Type.Should().Be(exampleItem.Type);
            outputIem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
    
     [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
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
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMembersList = _fixture.GetExampleCastMembersListWithNames([
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based onReal Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future"
        ]);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (CastMember outputIem in output.Items)
        {
            var exampleItem = exampleCastMembersList.Find(category => category.Id == outputIem.Id);
            exampleItem.Should().NotBeNull();
            outputIem!.Name.Should().Be(exampleItem!.Name);
            outputIem.Type.Should().Be(exampleItem.Type);
            outputIem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
    
     [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdat", "asc")]
    [InlineData("createdat", "desc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMemberList = _fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleCastMemberList);
        await dbContext.SaveChangesAsync();
        var castMemberRepository = new Repository.CastMemberRepository(dbContext);
        var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);

        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);
        var expectedOrderList = _fixture.CloneCastMembersListOrdered(exampleCastMemberList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCastMemberList.Count);
        output.Items.Should().HaveCount(exampleCastMemberList.Count);
        for (int i = 0; i < expectedOrderList.Count; i++)
        {
            var expectedItem = expectedOrderList[i];
            var outputItem = output.Items[i];
            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem!.Name.Should().Be(expectedItem!.Name);
            outputItem!.Id.Should().Be(expectedItem!.Id);
            outputItem!.CreatedAt.Should().Be(expectedItem!.CreatedAt);
            outputItem.Type.Should().Be(expectedItem.Type);
        }
    }
}