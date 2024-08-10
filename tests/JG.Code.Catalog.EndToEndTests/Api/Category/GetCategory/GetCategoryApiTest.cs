using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Category.Common;
using JG.Code.Catalog.EndToEndTests.Api.Category.CreateCategory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.GetCategoryById;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class GetCategoryApiTest
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public GetCategoryApiTest(CreateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task GetCategory()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture.ApiClient.Get<CategoryModelOutput>($"/categories/{exampleCategory.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task ThrowWhenNotFound()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var randonGuid = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>($"/categories/{randonGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category '{randonGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }
}
