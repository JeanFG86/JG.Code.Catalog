using FluentAssertions;
using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Repositories;
using JG.Code.Catalog.Application.UseCases.Category.ListCategories;
using AppUseCases = JG.Code.Catalog.Application.UseCases.Category.ListCategories;
using JG.Code.Catalog.Application.UseCases.Category.Common;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async Task SearchReturnsListAndTotal()
    {
        CodeCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(1, 20);
        var useCase = new AppUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        foreach (CategoryModelOutput outputIem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(category => category.Id == outputIem.Id);
            exampleItem.Should().NotBeNull();
            outputIem!.Name.Should().Be(exampleItem!.Name);
            outputIem.Description.Should().Be(exampleItem.Description);
            outputIem.IsActive.Should().Be(exampleItem.IsActive);
            outputIem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
}
