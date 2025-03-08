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
}