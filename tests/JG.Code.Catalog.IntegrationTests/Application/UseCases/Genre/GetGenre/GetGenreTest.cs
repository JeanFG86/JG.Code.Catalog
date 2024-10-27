using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Infra.Data.EF.Models;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.GetGenre;


namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var expectedGenre = genresExampleList[5];
        var dbArragneContext = _fixture.CreateDbContext();
        await dbArragneContext.Genres.AddRangeAsync(genresExampleList);
        await dbArragneContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var outPut = await useCase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.Id.Should().Be(expectedGenre.Id);
        outPut.Name.Should().Be(expectedGenre.Name);
        outPut.IsActive.Should().Be(expectedGenre.IsActive);
        outPut.CreatedAt.Should().Be(expectedGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(ThrowsWhenNotFound))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task ThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var randomGuid = Guid.NewGuid();
        var dbArragneContext = _fixture.CreateDbContext();
        await dbArragneContext.Genres.AddRangeAsync(genresExampleList);
        await dbArragneContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {randomGuid} not found.");
    }


    [Fact(DisplayName = nameof(GetGenreWithCategoryRelations))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreWithCategoryRelations()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var categoriesExampleList = _fixture.GetExampleCategoriesList(5);
        var expectedGenre = genresExampleList[5];
        categoriesExampleList.ForEach(category => expectedGenre.AddCategory(category.Id));
        var dbArragneContext = _fixture.CreateDbContext();
        await dbArragneContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbArragneContext.Genres.AddRangeAsync(genresExampleList);
        await dbArragneContext.GenresCategories.AddRangeAsync(expectedGenre.Categories.Select(categoryId => new GenresCategories(categoryId, expectedGenre.Id)));
        await dbArragneContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var outPut = await useCase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.Id.Should().Be(expectedGenre.Id);
        outPut.Name.Should().Be(expectedGenre.Name);
        outPut.IsActive.Should().Be(expectedGenre.IsActive);
        outPut.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        outPut.Categories.Should().HaveCount(expectedGenre.Categories.Count);
        outPut.Categories.ToList().ForEach(relationModel => 
        {
            expectedGenre.Categories.Should().Contain(relationModel.Id);
            relationModel.Name.Should().BeNull();
        });
    }
}
