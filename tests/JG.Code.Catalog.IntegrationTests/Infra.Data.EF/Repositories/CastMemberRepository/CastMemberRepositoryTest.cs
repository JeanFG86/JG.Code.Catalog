using FluentAssertions;
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

        var dbCategory = await (_fixture.CreateDbContext(true)).CastMembers.FindAsync(exampleCastMember.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCastMember.Name);
        dbCategory.Type.Should().Be(exampleCastMember.Type);
        dbCategory.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }
}