using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using Microsoft.AspNetCore.Http;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.GetCastMember;

[Collection(nameof(GetCastMemberApiTestFixture))]
public class GetCastMemberApiTest : IDisposable
{
    private readonly GetCastMemberApiTestFixture _fixture;

    public GetCastMemberApiTest(GetCastMemberApiTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(Get))]
    [Trait("EndToEnd/API", "CastMember/Get - Endpoints")]
    public async Task Get()
    {
        var examples = _fixture.GetExampleCastMembersList(20);
        var example = examples[10];
        await _fixture.Persistence.InsertList(examples);

        var (response, output) = await _fixture.ApiClient.Get<ApiResponse<CastMemberModelOutput>>($"/castmembers/{example.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Name.Should().Be(example.Name);
        output.Data.Type.Should().Be(example.Type);
    }
    
    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}