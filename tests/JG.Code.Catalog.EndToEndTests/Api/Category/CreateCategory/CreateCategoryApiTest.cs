using JG.Code.Catalog.Application.UseCases.Category.Common;
using DomianEntity = JG.Code.Catalog.Domain.Entity;
using FluentAssertions;
using System.Net;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.CreateCategory;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("EndToEnd/API", "CreateCategory - EndPoints")]
    public async Task CreateCategory()
    {
        var input = _fixture.GetExampleInput();

        var (reponse, output) = await _fixture.ApiClient.Post<CategoryModelOutput>("/categories", input);

        reponse.Should().NotBeNull();
        reponse!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        var dbCategory = await _fixture.Persistence.GetById(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.Id.Should().NotBeEmpty();
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }
}
