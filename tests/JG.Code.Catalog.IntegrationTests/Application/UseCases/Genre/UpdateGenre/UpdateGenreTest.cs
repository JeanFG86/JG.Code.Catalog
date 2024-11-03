using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Application.UseCases.Genre.UpdateGenre;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
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
}
