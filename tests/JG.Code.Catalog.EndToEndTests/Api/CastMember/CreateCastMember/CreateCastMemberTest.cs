using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using JG.Code.Catalog.Application.UseCases.CastMember.CreateCastMember;
using Microsoft.AspNetCore.Http;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest : IDisposable
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("EndToEnd/API", "CastMember/Create - Endpoints")]
    public async Task CreateCastMember()
    {
        var apiInput = new CreateCastMemberInput(_fixture.GetValidName(), _fixture.GetRandomCastMemberType());
        
        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<CastMemberModelOutput>>($"/castmembers", apiInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output!.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.Type.Should().Be(apiInput.Type);
        var castMemberFromDb = await _fixture.Persistence.GetById(output!.Data.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Name.Should().Be(apiInput.Name);
    }
    
    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}