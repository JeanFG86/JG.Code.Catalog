using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Application.UseCases.Genre.UpdateGenre;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Models;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Intregation/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres();
        CodeCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.SaveChangesAsync();
        CodeCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
        UpdateGenreInput input = new UpdateGenreInput(targetGenre.Id, _fixture.GetValidGenreName(), !targetGenre.IsActive);

        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        CodeCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    [Trait("Intregation/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithRelations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList();
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres();
        CodeCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
        UpdateGenreInput input = new UpdateGenreInput(targetGenre.Id, _fixture.GetValidGenreName(), !targetGenre.IsActive, newRelatedCategories.Select(x => x.Id).ToList());

        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(newRelatedCategories.Count);
        List<Guid> relatedCategoriesIdsFromOutput = output.Categories.Select(x => x.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
        CodeCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoryIdsFromDb = await assertDbContext.GenresCategories.AsNoTracking().Where(relation => relation.GenreId == input.Id).Select(relation => relation.CategoryId).ToListAsync();
        relatedCategoryIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutNewRelations))]
    [Trait("Intregation/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutNewRelations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList();
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres();
        CodeCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
        UpdateGenreInput input = new UpdateGenreInput(targetGenre.Id, _fixture.GetValidGenreName(), !targetGenre.IsActive);

        GenreModelOutput output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(relatedCategories.Count);
        List<Guid> expectedRelatedCategoryIds = relatedCategories.Select(c => c.Id).ToList();
        List<Guid> relatedCategoriesIdsFromOutput = output.Categories.Select(x => x.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(expectedRelatedCategoryIds);
        CodeCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedCategoryIdsFromDb = await assertDbContext.GenresCategories.AsNoTracking().Where(relation => relation.GenreId == input.Id).Select(relation => relation.CategoryId).ToListAsync();
        relatedCategoryIdsFromDb.Should().BeEquivalentTo(expectedRelatedCategoryIds);
    }

    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategoryDoesntExists))]
    [Trait("Intregation/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreThrowsWhenCategoryDoesntExists()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList();
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres();
        CodeCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories.Select(categoryId => new GenresCategories(categoryId, targetGenre.Id)).ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
        List<Guid> categoriesIdsToRelate = newRelatedCategories.Select(category => category.Id).ToList();
        Guid invalidCategoryId = Guid.NewGuid();
        categoriesIdsToRelate.Add(invalidCategoryId);
        UpdateGenreInput input = new UpdateGenreInput(targetGenre.Id, _fixture.GetValidGenreName(), !targetGenre.IsActive, categoriesIdsToRelate);

        Func<Task<GenreModelOutput>> action = async () => await updateGenre.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category id (or ids) not found: {invalidCategoryId}");
    }

    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenNotFound))]
    [Trait("Intregation/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreThrowsWhenNotFound()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres();
        CodeCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.SaveChangesAsync();
        CodeCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
        Guid randomGuid = Guid.NewGuid();
        UpdateGenreInput input = new UpdateGenreInput(randomGuid, _fixture.GetValidGenreName(), true);

        Func<Task<GenreModelOutput>> action = async () => await updateGenre.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {randomGuid} not found.");
    }
}
