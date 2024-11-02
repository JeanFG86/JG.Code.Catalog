using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Application.Exceptions;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        CreateGenreInput input = _fixture.GetExampleInput();
        var actDbContext = _fixture.CreateDbContext();
        var useCase = new UseCase.CreateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

        var outPut = await useCase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.Name.Should().Be(input.Name);
        outPut.IsActive.Should().Be(input.IsActive);
        outPut.CreatedAt.Should().NotBe(default);
        outPut.Categories.Should().HaveCount(0);
        var asserDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await asserDbContext.Genres.FindAsync(outPut.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
    }

    [Fact(DisplayName = nameof(CreateGenreWithCategoriesRelations))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenreWithCategoriesRelations()
    {        
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(5);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();
        CreateGenreInput input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories.Select(c => c.Id).ToList();
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.CreateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

        var outPut = await useCase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.Name.Should().Be(input.Name);
        outPut.IsActive.Should().Be(input.IsActive);
        outPut.CreatedAt.Should().NotBe(default);
        outPut.Categories.Should().HaveCount(input.CategoriesIds.Count);
        outPut.Categories.Select(relation => relation.Id).Should().BeEquivalentTo(input.CategoriesIds);
        var asserDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await asserDbContext.Genres.FindAsync(outPut.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
        List<GenresCategories> relations = await asserDbContext.GenresCategories.AsNoTracking().Where(x => x.GenreId == outPut.Id).ToListAsync();
        relations.Should().HaveCount(input.CategoriesIds.Count);
        List<Guid> categoriesIdsRelatedFromDb = relations.Select(relation => relation.CategoryId).ToList();
        categoriesIdsRelatedFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Fact(DisplayName = nameof(CreateGenreThrowsWhenCategoryDoesntExists))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenreThrowsWhenCategoryDoesntExists()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(5);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();
        CreateGenreInput input = _fixture.GetExampleInput();
        input.CategoriesIds = exampleCategories.Select(c => c.Id).ToList();
        Guid randomGuid = Guid.NewGuid();
        input.CategoriesIds.Add(randomGuid);
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.CreateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

        Func<Task<GenreModelOutput>> action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id (or ids) not found: {randomGuid}");
    }
}
