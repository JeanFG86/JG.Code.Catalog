using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Category.CreateCategory;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using UsesCases = JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("Integration/Application", "CreateCastMember - Use Cases")]
    public async Task CreateCastMember()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UsesCases.CreateCastMember(repository, unitOfWork);
        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbCastMember = await (_fixture.CreateDbContext(true)).CastMembers.FindAsync(output.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(input.Name);
        dbCastMember.Type.Should().Be(input.Type);
        dbCastMember.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }
    
    [Theory(DisplayName = nameof(ThrowsWhenInvalidName))]
    [Trait("Integration/Application", "CreateCastMember - Use Cases")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ThrowsWhenInvalidName(string? name)
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UsesCases.CreateCastMember(repository, unitOfWork);
        var exampleInput = new UsesCases.CreateCastMemberInput(name, _fixture.GetRandomCastMemberType());

        var task = async () => await useCase.Handle(exampleInput, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>().WithMessage("Name should not be empty or null");
        var dbCategoriesList = _fixture.CreateDbContext().Categories.AsNoTracking().ToList();
        dbCategoriesList.Should().HaveCount(0);
    }
}