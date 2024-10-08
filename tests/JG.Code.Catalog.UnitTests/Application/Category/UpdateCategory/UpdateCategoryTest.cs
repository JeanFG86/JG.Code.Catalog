﻿using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.UseCases.Category.Common;
using JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;
using JG.Code.Catalog.Domain.Exceptions;
using Moq;
using UseCase = JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Application.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryDataGenerator)
    )]
    public async Task UpdateCategory(DomainEntity.Category exampleCategory, UpdateCategoryInput input)
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Update(exampleCategory, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var input = _fixture.GetValidInput();
        repositoryMock.Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"category '{input.Id}' not found"));
        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);


        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(repository => repository.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Update(It.IsAny<DomainEntity.Category>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutProvidingIsActive))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryDataGenerator)
    )]
    public async Task UpdateCategoryWithoutProvidingIsActive(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Update(exampleCategory, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive!);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryDataGenerator)
    )]
    public async Task UpdateCategoryOnlyName(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Update(exampleCategory, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive!);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateCategory))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryDataGenerator.GetInvalidInputs), parameters: 12, MemberType = typeof(UpdateCategoryDataGenerator))]
    public async Task ThrowWhenCantUpdateCategory(UpdateCategoryInput input, string exceptionMessage)
    {
        var exampleCategory = _fixture.GetExampleCategory();
        input.Id = exampleCategory.Id;
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>().WithMessage(exceptionMessage);
        repositoryMock.Verify(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once());
    }
}
