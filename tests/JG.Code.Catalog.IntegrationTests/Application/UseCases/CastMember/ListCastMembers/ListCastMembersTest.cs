using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
using JG.Code.Catalog.Infra.Data.EF.Repositories;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMembers;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.CastMember.ListCastMembers;


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
}