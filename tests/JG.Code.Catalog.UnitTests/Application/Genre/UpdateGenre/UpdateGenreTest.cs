using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.UpdateGenre;
using JG.Code.Catalog.Application.UseCases.Category.Common;
using JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;
using Moq;
using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Genre.Common;

namespace JG.Code.Catalog.UnitTests.Application.Genre.UpdateGenre;


[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest 
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var newNameExample =_fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(repository => repository.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(repository => repository.Update(exampleGenre, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(0);
    }
}
