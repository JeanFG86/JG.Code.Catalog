﻿using UseCase = JG.Code.Catalog.Application.UseCases.Genre.UpdateGenre;
using Moq;
using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Exceptions;

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
}