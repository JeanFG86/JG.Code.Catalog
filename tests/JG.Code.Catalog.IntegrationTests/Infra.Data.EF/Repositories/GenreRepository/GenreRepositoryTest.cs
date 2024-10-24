using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Repository = JG.Code.Catalog.Infra.Data.EF.Repositories;

namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Insert()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var genreRepository = new Repository.GenreRepository(dbContext);
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.SaveChangesAsync();

        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = await assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();
        genreCategoriesRelation.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelation.ForEach(r =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == r.CategoryId);
            expectedCategory.Should().NotBeNull();
        });
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Get()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();        
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync();

        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));
        var genreFromRepository = await genreRepository.Get(exampleGenre.Id, CancellationToken.None);
    
        genreFromRepository.Should().NotBeNull();
        genreFromRepository!.Name.Should().Be(exampleGenre.Name);
        genreFromRepository.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromRepository.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreFromRepository.Categories.Should().HaveCount(categoriesListExample.Count);
        foreach (var categoryId in genreFromRepository.Categories)
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == categoryId);
            expectedCategory.Should().NotBeNull();            
        }
    }

    [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetThrowWhenNotFound()
    {
        var exampleNotFoundGuid = Guid.NewGuid();
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync();

        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));
        var action = async () => await genreRepository.Get(exampleNotFoundGuid, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {exampleNotFoundGuid} not found.");
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Delete()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync();

        var repositoryDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(repositoryDbContext);
        await genreRepository.Delete(exampleGenre, CancellationToken.None);
        await repositoryDbContext.SaveChangesAsync();

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbGenre = assertDbContext.Genres.AsNoTracking().FirstOrDefault(x => x.Id == exampleGenre.Id);
        dbGenre.Should().BeNull();
        var categoriesIdsList = assertDbContext.GenresCategories.AsNoTracking().Where(x => x.GenreId == exampleGenre.Id).Select(x => x.CategoryId).ToList();
        categoriesIdsList.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Update()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        
        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else 
            exampleGenre.Activate();
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = await assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();
        genreCategoriesRelation.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelation.ForEach(r =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == r.CategoryId);
            expectedCategory.Should().NotBeNull();
        });
    }

    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateRemovingRelations()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = await assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();
        genreCategoriesRelation.Should().HaveCount(0);       
    }

    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateReplacingRelations()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        var updateCategoriesListExample = _fixture.GetExampleCategoriesList(2);
        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Categories.AddRangeAsync(updateCategoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        updateCategoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelation = await assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();
        genreCategoriesRelation.Should().HaveCount(updateCategoriesListExample.Count);
        genreCategoriesRelation.ForEach(r =>
        {
            var expectedCategory = updateCategoriesListExample.FirstOrDefault(x => x.Id == r.CategoryId);
            expectedCategory.Should().NotBeNull();
        });
    }

    [Fact(DisplayName = nameof(ListReturnsItemsAndTotal))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task ListReturnsItemsAndTotal()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenres();       
        await dbContext.Genres.AddRangeAsync(exampleGenresList);       
        await dbContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);
       
        var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (var resultItem in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == resultItem.Id);
            exampleGenre.Should().NotBeNull();
            resultItem.Name.Should().Be(resultItem.Name);
            resultItem.IsActive.Should().Be(resultItem.IsActive);
            resultItem.CreatedAt.Should().Be(resultItem.CreatedAt);
        }
    }
}
