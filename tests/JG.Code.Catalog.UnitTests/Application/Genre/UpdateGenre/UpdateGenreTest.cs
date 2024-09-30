using UseCase = JG.Code.Catalog.Application.UseCases.Genre.UpdateGenre;
using Moq;
using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Exceptions;
using System.Linq;

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

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var exampleId = Guid.NewGuid();
        genreRepositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found."));
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, _fixture.GetUnitOfWorkMock().Object, _fixture.GetCategoryRepositoryMock().Object);
        var input = new UseCase.UpdateGenreInput(exampleId, _fixture.GetValidGenreName(), true);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{exampleId}' not found.");
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ThrowWhenNameIsInvalid(string? name)
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, name!, newIsActive);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>().WithMessage($"Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreOnlyName(bool isActive)
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(isActive);
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(repository => repository.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(repository => repository.Update(exampleGenre, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(isActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(UpdateGenreAddingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreAddingCategoriesIds()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategoriesIdsList = _fixture.GetRandonIdsList();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategoriesIdsList);

        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive, exampleCategoriesIdsList);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(repository => repository.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(repository => repository.Update(exampleGenre, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
        exampleCategoriesIdsList.ForEach(expectedId => output.Categories.Should().Contain(expectedId));
    }

    [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreReplacingCategoriesIds()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandonIdsList());
        var exampleCategoriesIdsList = _fixture.GetRandonIdsList();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategoriesIdsList);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive, exampleCategoriesIdsList);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(repository => repository.Get(exampleGenre.Id, It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(repository => repository.Update(exampleGenre, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
        exampleCategoriesIdsList.ForEach(expectedId => output.Categories.Should().Contain(expectedId));
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandonIdsList());
        var exampleNewCategoriesIdsList = _fixture.GetRandonIdsList();
        var listReturnedByCategoryRepository = exampleNewCategoriesIdsList.GetRange(0, exampleNewCategoriesIdsList.Count - 2);
        var idsNotReturnedByCategoryRepository = exampleNewCategoriesIdsList.GetRange(exampleNewCategoriesIdsList.Count -2, 2);
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(listReturnedByCategoryRepository);
        var useCase = new UseCase.UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample, newIsActive, exampleNewCategoriesIdsList);

        var action = async() => await useCase.Handle(input, CancellationToken.None);

        var notFoundIdsAsString = String.Join(", ", idsNotReturnedByCategoryRepository);
        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id (or ids) not found: {notFoundIdsAsString}");
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutCategoriesIds()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleCategoriesIdsList = _fixture.GetRandonIdsList();
        var exampleGenre = _fixture.GetExampleGenre(categoriesIds: exampleCategoriesIdsList);        
        var newNameExample = _fixture.GetValidGenreName();
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
        output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
        exampleCategoriesIdsList.ForEach(expectedId => output.Categories.Should().Contain(expectedId));
    }
}
