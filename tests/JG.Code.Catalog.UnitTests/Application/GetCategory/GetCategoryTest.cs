using JG.Code.Catalog.Domain.Entity;
using useCase = JG.Code.Catalog.Application.UseCases.Category.GetCategory;
using Moq;
using FluentAssertions;

namespace JG.Code.Catalog.UnitTests.Application.GetCategory;

[Collection(nameof(GetCategoryFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryFixture _fixture;

    public GetCategoryTest(GetCategoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async Task GetCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleCategory = _fixture.GetValidCategory();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
        var input = new useCase.GetCategoryInput(exampleCategory.Id);
        var useCase = new useCase.GetCategory(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.Id.Should().Be(exampleCategory.Id);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }
}
