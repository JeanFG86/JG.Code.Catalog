using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Category.CreateCategory;
using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.UnitTests.Domain.Entity.Category;
using Moq;
using UsesCases = JG.Code.Catalog.Application.UseCases.Category.CreateCategory;

namespace JG.Code.Catalog.UnitTests.Application.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UsesCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateAggregate))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(nameof(GetInvalidInputs))]
    public async void ThrowWhenCantInstantiateAggregate(CreateCategoryInput input, string exceptionMessage)
    {
        var useCase = new UsesCases.CreateCategory(_fixture.GetRepositoryMock().Object, _fixture.GetUnitOfWorkMock().Object);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);
        await task.Should().ThrowAsync<EntityValidationException>().WithMessage(exceptionMessage);
    }

    public static IEnumerable<Object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputList = new List<Object[]>();

        var invalidInputShortName = fixture.GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        invalidInputList.Add(
        [
            invalidInputShortName,
            "Name should be at least 3 characters long"
        ]);

        var invalidInputTooLongName = fixture.GetInput();
        var tooLongNameForCategory = fixture.Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)        
            tooLongNameForCategory = $"{tooLongNameForCategory} {fixture.Faker.Commerce.ProductName()}";
        
        invalidInputTooLongName.Name = tooLongNameForCategory;
        invalidInputList.Add(
        [
            invalidInputTooLongName,
            "Name should be less or equal 255 characters"
        ]);

        var invalidInputDescriptionNull = fixture.GetInput();
        invalidInputDescriptionNull.Description = null;
        invalidInputList.Add(
        [
            invalidInputDescriptionNull,
            "Description should not be null"
        ]);

        var invalidInputTooLongDescription = fixture.GetInput();
        var tooLongDescriptionForCategory = fixture.Faker.Commerce.ProductDescription();
        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {fixture.Faker.Commerce.ProductName()}";
        invalidInputTooLongDescription.Description = tooLongDescriptionForCategory;
        invalidInputList.Add(
        [
            invalidInputTooLongDescription,
            "Description should be less or equal 10000 characters"
        ]);
        return invalidInputList;
    }
}
