using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Repositories;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.UpdateCastMember;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.CastMember.UpdateCastMember;


[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest
{
    private readonly UpdateCastMemberTestFixture _fixture;

    public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof( UpdateCastMember))]
    [Trait("Intregation/Application", "UpdateCastMember - Use Cases")]
    public async Task  UpdateCastMember()
    {
        List<DomainEntity.CastMember> exampleCastMembers = _fixture.GetExampleCastMembersList();
        CodeCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.CastMember targetCastMember = exampleCastMembers[5];
        await arrangeDbContext.AddRangeAsync(exampleCastMembers);
        await arrangeDbContext.SaveChangesAsync();
        CodeCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.UpdateCastMember updateCastMember = new UseCase.UpdateCastMember(new CastMemberRepository(actDbContext), new UnitOfWork(actDbContext));
        UseCase.UpdateCastMemberInput input = new UseCase.UpdateCastMemberInput(targetCastMember.Id, _fixture.GetValidName(), _fixture.GetRandomCastMemberType());

        CastMemberModelOutput output = await updateCastMember.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetCastMember.Id);
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        CodeCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.CastMember? genreFromDb = await assertDbContext.CastMembers.FindAsync(targetCastMember.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetCastMember.Id);
        genreFromDb.Name.Should().Be(input.Name);
    }
}