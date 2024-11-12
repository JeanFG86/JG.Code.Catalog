using FluentAssertions;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.DeleteGenre;
using Microsoft.EntityFrameworkCore;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Application.Exceptions;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public  class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var targetGenre = genresExampleList[5];
        var dbArragneContext = _fixture.CreateDbContext();
        await dbArragneContext.Genres.AddRangeAsync(genresExampleList);
        await dbArragneContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.DeleteGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext));
        var input = new UseCase.DeleteGenreInput(targetGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleListGenres();
        var targetGenre = genresExampleList[5];
        var dbArragneContext = _fixture.CreateDbContext();
        await dbArragneContext.Genres.AddRangeAsync(genresExampleList);
        await dbArragneContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.DeleteGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext));
        var randomGuid = Guid.NewGuid();
        var input = new UseCase.DeleteGenreInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {randomGuid} not found.");
    }
}
