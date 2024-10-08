using UseCase = JG.Code.Catalog.Application.UseCases.Genre.DeleteGenre;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using Moq;
using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;

namespace JG.Code.Catalog.UnitTests.Application.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var repositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var genreExample = _fixture.GetExampleGenre();
        repositoryMock.Setup(x => x.Get(genreExample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(genreExample);
        var input = new UseCase.DeleteGenreInput(genreExample.Id);
        var useCase = new UseCase.DeleteGenre(repositoryMock.Object, unitOfWorkMock.Object);

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(genreExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Delete(genreExample, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWheGenreNotFound))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task ThrowWheGenreNotFound()
    {
        var repositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(x => x.Get(exampleGuid, It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Genre '{exampleGuid}' not found"));
        var input = new UseCase.DeleteGenreInput(exampleGuid);
        var useCase = new UseCase.DeleteGenre(repositoryMock.Object, unitOfWorkMock.Object);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(repository => repository.Get(exampleGuid, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Delete(It.IsAny<DomainEntity.Genre>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}
