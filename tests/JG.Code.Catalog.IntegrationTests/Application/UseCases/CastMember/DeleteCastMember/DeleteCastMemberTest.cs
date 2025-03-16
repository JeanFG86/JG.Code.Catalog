using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using AppUseCase = JG.Code.Catalog.Application.UseCases.CastMember.DeleteCastMember;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.DeleteCastMember;

[Collection(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTest
{
    private readonly DeleteCastMemberTestFixture _fixture;
    public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(DeleteCastMember))]
    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    public async Task DeleteCastMember()
    {
        var dbContext = _fixture.CreateDbContext();
        var castMemberExample = _fixture.GetExampleCastMember();
        var exampleList = _fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleList);
        var trackingInfo = await dbContext.AddAsync(castMemberExample);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;
        var repository = new CastMemberRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new AppUseCase.DeleteCastMember(repository, unitOfWork);
        var input = new DeleteCastMemberInput(castMemberExample.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbCastMemberDeleted = await assertDbContext.CastMembers.FindAsync(castMemberExample.Id);
        dbCastMemberDeleted.Should().BeNull();
        var dbCastMembers = await assertDbContext.CastMembers.ToListAsync();
        dbCastMembers.Should().HaveCount(exampleList.Count);
    }
    
    [Fact(DisplayName = nameof(ThrowWhenCastMemberNotFound))]
    [Trait("Integration/Application", "DeleteCastMember - Use Cases")]
    public async Task ThrowWhenCastMemberNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleList = _fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync();
        var repository = new CastMemberRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new AppUseCase.DeleteCastMember(repository, unitOfWork);
        var input = new DeleteCastMemberInput(Guid.NewGuid());

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"CastMember '{input.Id}' not found.");
    }
}