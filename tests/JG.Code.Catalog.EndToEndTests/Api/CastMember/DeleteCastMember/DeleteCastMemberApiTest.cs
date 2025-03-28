using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.DeleteCastMember;

[Collection(nameof(DeleteCastMemberApiTestFixture))]
public class DeleteCastMemberApiTest : IDisposable
{
    private readonly DeleteCastMemberApiTestFixture _fixture;

    public DeleteCastMemberApiTest(DeleteCastMemberApiTestFixture fixture)
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

    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}
