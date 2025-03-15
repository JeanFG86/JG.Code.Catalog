using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using UseCase = JG.Code.Catalog.Application.UseCases.CastMember.GetCastMember;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.GetCastMember;

[Collection(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTest
{
    private readonly GetCastMemberTestFixture _fixture;

    public GetCastMemberTest(GetCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(GetCastMember))]
    [Trait("Integration/Application", "GetCastMember - Use Cases")]
    public async Task GetCastMember()
    {
        var exampleCastMember = _fixture.GetExampleCastMember();
        var dbContext = _fixture.CreateDbContext();
        dbContext.CastMembers.Add(exampleCastMember);
        await dbContext.SaveChangesAsync();
        var repository = new CastMemberRepository(dbContext);                
        var input = new UseCase.GetCastMemberInput(exampleCastMember.Id);
        var useCase = new UseCase.GetCastMember(repository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(exampleCastMember.Name);
        output.Id.Should().Be(exampleCastMember.Id);
        output.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }
    
    [Fact(DisplayName = nameof(NotFoundExceptionWhenCastMemberDoesntExist))]
    [Trait("Integration/Application", "GetCastMember - Use Cases")]
    public async Task NotFoundExceptionWhenCastMemberDoesntExist()
    {
        var exampleCastMember = _fixture.GetExampleCastMember();
        var dbContext = _fixture.CreateDbContext();
        dbContext.CastMembers.Add(exampleCastMember);
        await dbContext.SaveChangesAsync();
        var repository = new CastMemberRepository(dbContext);
        var input = new UseCase.GetCastMemberInput(Guid.NewGuid());
        var useCase = new UseCase.GetCastMember(repository);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
    }
}