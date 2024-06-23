using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Repository;
using Moq;
using UsesCases = JG.Code.Catalog.Application.UseCases;

namespace JG.Code.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTest
{
    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UsesCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
        var input = new CreateCategoryInput("Category Name", "Category Description", true);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be("Category Name");
        output.Description.Should().Be("Category Description");
        output.IsActive.Should().Be(true);
        (output.Id != null && output.Id != Guid.Empty).Should().Be(true);
        (output.CreatedAt != null && output.CreatedAt != default(DateTime)).Should().Be(true);
    }
}
