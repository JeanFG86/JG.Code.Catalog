using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreApiTestFixture))]
public class DeleteGenreApiTest
{
    private readonly DeleteGenreApiTestFixture _fixture;

    public DeleteGenreApiTest(DeleteGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
    public async Task DeleteGenre()
    {
        var exampleGenresList = _fixture.GetExampleListGenres(10);
        await _fixture.Persistence.InsertList(exampleGenresList);
        var exampleGenre = exampleGenresList[5];

        var (response, output) = await _fixture.ApiClient.Delete<object>($"/genres/{exampleGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();
        var pessistenceGenre = await _fixture.Persistence.GetById(exampleGenre.Id);
        pessistenceGenre.Should().BeNull();
    }

    [Fact(DisplayName = nameof(WhenNotFound404))]
    [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
    public async Task WhenNotFound404()
    {
        var exampleGenresList = _fixture.GetExampleListGenres(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.InsertList(exampleGenresList);

        var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>($"/genres/{randomGuid}");

        response.Should().NotBeNull();
        output.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre {randomGuid} not found.");
    }
}