using JG.Code.Catalog.Infra.Data.EF.Repositories;
using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using AppUseCase = JG.Code.Catalog.Application.UseCases.Category.DeleteCategory;
using JG.Code.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var categoryExample = _fixture.GetExampleCategory();
        var exampleList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleList);
        var trackingInfo = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new AppUseCase.DeleteCategory(repository, unitOfWork);
        var input = new DeleteCategoryInput(categoryExample.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var dbCategoryDeleted = await assertDbContext.Categories.FindAsync(categoryExample.Id);
        dbCategoryDeleted.Should().BeNull();
        var dbCategories = await assertDbContext.Categories.ToListAsync();
        dbCategories.Should().HaveCount(exampleList.Count);
    }

}
