using Moq;
using UsesCases = JG.Code.Catalog.Application.UseCases.Category.DeleteCategory;

namespace JG.Code.Catalog.UnitTests.Application.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryExample = _fixture.GetValidCategory();
        repositoryMock.Setup(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(categoryExample);
        var input = new UsesCases.DeleteCategoryInput(categoryExample.Id);
        var useCase = new UsesCases.DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(categoryExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(repository => repository.Delete(categoryExample, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
