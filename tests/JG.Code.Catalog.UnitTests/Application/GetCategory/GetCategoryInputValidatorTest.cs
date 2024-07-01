using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Category.GetCategory;

namespace JG.Code.Catalog.UnitTests.Application.GetCategory;

[Collection(nameof(GetCategoryFixture))]
public class GetCategoryInputValidatorTest
{
    private readonly GetCategoryFixture _fixture;

    public GetCategoryInputValidatorTest(GetCategoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application", "GetCategoryInputValidation - Use Cases")]
    public void ValidationOk()
    {
        var validInput = new GetCategoryInput(Guid.NewGuid());
        var validator = new GetCategoryInputValidator();

        var validationResult = validator.Validate(validInput);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().HaveCount(0);
    }
}
