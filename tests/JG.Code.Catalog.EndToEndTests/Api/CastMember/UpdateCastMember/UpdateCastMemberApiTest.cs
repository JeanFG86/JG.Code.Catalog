﻿using System.Net;
using FluentAssertions;
using JG.Code.Catalog.Api.ApiModels.CastMember;
using JG.Code.Catalog.Api.ApiModels.Response;
using JG.Code.Catalog.Application.UseCases.CastMember.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember;

[Collection(nameof(UpdateCastMemberApiTestFixture))]
public class UpdateCastMemberApiTest: IDisposable
{
    private readonly UpdateCastMemberApiTestFixture _fixture;

    public UpdateCastMemberApiTest(UpdateCastMemberApiTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(UpdateCastMember))]
    [Trait("EndToEnd/API", "CastMember/Update - Endpoints")]
    public async Task UpdateCastMember()
    {
        var examples = _fixture.GetExampleCastMembersList(10);
        var target = examples[5];
        await _fixture.Persistence.InsertList(examples);
        var input = new UpdateCastMemberApiInput(_fixture.GetValidName(), _fixture.GetRandomCastMemberType());

        var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CastMemberModelOutput>>($"/castmembers/{target.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(target.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Type.Should().Be(input.Type);
        var castMemberFromDb = await _fixture.Persistence.GetById(output!.Data.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Name.Should().Be(input.Name);
        castMemberFromDb.Type.Should().Be(input.Type);
    }
    
    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "CastMember/Update - EndPoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleList = _fixture.GetExampleCastMembersList(20);
        await _fixture.Persistence.InsertList(exampleList);
        var randomGuid = Guid.NewGuid();
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/castmembers/{randomGuid}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }
    
    public void Dispose()
    {
        _fixture.CleanPersistence();
    }
}