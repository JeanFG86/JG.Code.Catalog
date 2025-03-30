using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.DeleteCastMember;

[Collection(nameof(DeleteCastMemberApiApiTestFixture))]
public class DeleteCastMemberApiTest : IDisposable
{
    private readonly DeleteCastMemberApiApiTestFixture _fixture;

    public DeleteCastMemberApiTest(DeleteCastMemberApiApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCastMember))]
    [Trait("EndToEnd/API", "CastMember/Delete - Endpoints")]
    public async Task DeleteCastMember()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(20);
        await _fixture.Persistence.InsertList(exampleCastMembersList);
        var exampleCastMember = exampleCastMembersList[10];

        var (response, output) = await _fixture.ApiClient.Delete<object>($"/castmembers/{exampleCastMember.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();
        var pessistenceCategory = await _fixture.Persistence.GetById(exampleCastMember.Id);
        pessistenceCategory.Should().BeNull();
    }
    
    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "CastMember/Delete - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCastMembersList = _fixture.GetExampleCastMembersList(20);
        await _fixture.Persistence.InsertList(exampleCastMembersList);
        var exampleRandonGuid = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>($"/castmembers/{exampleRandonGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{exampleRandonGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
