using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Category.Common;
using JG.Code.Catalog.EndToEndTests.Api.Category.CreateCategory;
using Microsoft.AspNetCore.Http;
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
}
