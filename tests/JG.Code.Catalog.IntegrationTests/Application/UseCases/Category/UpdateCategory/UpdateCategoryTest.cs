﻿using JG.Code.Catalog.Application.UseCases.Category.Common;
using JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using JG.Code.Catalog.Infra.Data.EF;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using ApplicationUseCase = JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using JG.Code.Catalog.Application.Exceptions;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryDataGenerator)
    )]
    public async Task UpdateCategory(DomainEntity.Category exampleCategory, UpdateCategoryInput input)
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached;
        var useCase = new ApplicationUseCase.UpdateCategory(repository, unitOfWork);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive!);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);        
    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")] 
    [MemberData(
        nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryDataGenerator)
    )]
    public async Task UpdateCategoryWithoutIsActive(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached;
        var useCase = new ApplicationUseCase.UpdateCategory(repository, unitOfWork);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryDataGenerator)
    )]
    public async Task UpdateCategoryOnlyName(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trackingInfo = await dbContext.AddAsync(exampleCategory);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached;
        var useCase = new ApplicationUseCase.UpdateCategory(repository, unitOfWork);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
    }


    [Fact(DisplayName = nameof(UpdateThrowsWhenNotFoundCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    public async Task UpdateThrowsWhenNotFoundCategory()
    {
        var input = _fixture.GetValidInput();
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        dbContext.SaveChanges();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCase.UpdateCategory(repository, unitOfWork);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{input.Id}' not found.");
    }
}
