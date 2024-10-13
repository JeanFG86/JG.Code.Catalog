using FluentAssertions;
using JG.Code.Catalog.Infra.Data.EF;
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
}
