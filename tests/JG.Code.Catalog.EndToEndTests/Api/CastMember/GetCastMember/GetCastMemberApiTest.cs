using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.GetCastMember;

[Collection(nameof(GetCastMemberApiApiTestFixture))]
public class GetCastMemberApiTest : IDisposable
{
    private readonly GetCastMemberApiApiTestFixture _fixture;

    public GetCastMemberApiTest(GetCastMemberApiApiTestFixture fixture)
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
    
    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "CastMember/Get - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var examples = _fixture.GetExampleCastMembersList(20);
        await _fixture.Persistence.InsertList(examples);
        var randonGuid = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>($"/castmembers/{randonGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{randonGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }
    
    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}