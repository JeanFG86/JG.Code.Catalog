using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

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
        var exampleGenre = exampleGenresList[10];

        var (response, output) = await _fixture.ApiClient.Delete<object>($"/genres/{exampleGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();
        var pessistenceGenre = await _fixture.Persistence.GetById(exampleGenre.Id);
        pessistenceGenre.Should().BeNull();
    }

}