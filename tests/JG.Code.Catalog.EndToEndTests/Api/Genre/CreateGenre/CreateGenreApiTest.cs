using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
using Microsoft.AspNetCore.Http;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.CreateGenre;

[Collection(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTest
{
    private readonly CreateGenreApiTestFixture _fixture;

    public CreateGenreApiTest(CreateGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("EndToEnd/API", "Genre/Create - Endpoints")]
    public async Task CreateGenre()
    {
        var apiInput = new CreateGenreInput(_fixture.GetValidCategoryName(), _fixture.GetRandomBoolean());
        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<GenreModelOutput>>($"/genres", apiInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output!.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.IsActive.Should().Be(apiInput.IsActive);
        output.Data.Categories.Should().HaveCount(0);
        var genreFromDb = await _fixture.Persistence.GetById(output!.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(apiInput.Name);
        genreFromDb.IsActive.Should().Be(apiInput.IsActive);
    }
}