using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using Microsoft.AspNetCore.Http;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.GetGenre;
using DomainEntity = Domain.Entity;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest
{
    private readonly GetGenreApiTestFixture _fixture;

    public GetGenreApiTest(GetGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("EndToEnd/API", "Genre/Get - Endpoints")]
    public async Task GetGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
    }
}