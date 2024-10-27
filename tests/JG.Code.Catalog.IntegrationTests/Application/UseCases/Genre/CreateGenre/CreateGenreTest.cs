using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;

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
}
