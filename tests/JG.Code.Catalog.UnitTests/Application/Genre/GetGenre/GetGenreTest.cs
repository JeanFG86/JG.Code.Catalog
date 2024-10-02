using FluentAssertions;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using Moq;
using UseCase = JG.Code.Catalog.Application.UseCases.Genre.GetGenre;

namespace JG.Code.Catalog.UnitTests.Application.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandonIdsList());       
        genreRepositoryMock.Setup(x => x.Get(exampleGenre.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleGenre);
        var useCase = new UseCase.GetGenre(genreRepositoryMock.Object);
        var input = new UseCase.GetGenreInput(exampleGenre.Id);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(repository => repository.Get(It.Is<Guid>(x => x == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        foreach (var expectedId in exampleGenre.Categories)        
            output.Categories.Should().Contain(expectedId);        
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var exampleId = Guid.NewGuid();
        genreRepositoryMock.Setup(x => x.Get(It.Is<Guid>(x => x == exampleId), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found"));
        var useCase = new UseCase.GetGenre(genreRepositoryMock.Object);
        var input = new UseCase.GetGenreInput(exampleId);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre '{exampleId}' not found");
        genreRepositoryMock.Verify(x => x.Get(It.Is<Guid>(x => x == exampleId), It.IsAny<CancellationToken>()), Times.Once);        
    }
}
