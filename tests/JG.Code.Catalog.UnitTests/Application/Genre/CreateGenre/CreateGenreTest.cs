using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UsesCases = JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
using Moq;
using FluentAssertions;

namespace JG.Code.Catalog.UnitTests.Application.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UsesCases.CreateGenre(repositoryMock.Object, unitOfWorkMock.Object);
        var input = _fixture.GetExampleInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Insert(It.IsAny<DomainEntity.Genre>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().HaveCount(0);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }
}
