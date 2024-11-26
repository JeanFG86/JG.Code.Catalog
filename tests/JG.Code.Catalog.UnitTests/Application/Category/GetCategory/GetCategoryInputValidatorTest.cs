using System.Globalization;
using FluentAssertions;
using JG.Code.Catalog.Application.UseCases.Category.GetCategory;

namespace JG.Code.Catalog.UnitTests.Application.Category.GetCategory;

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

    [Fact(DisplayName = nameof(InvalidWhenEmptyGuidId))]
    [Trait("Application", "GetCategoryInputValidation - Use Cases")]
    public void InvalidWhenEmptyGuidId()
    {
        // Define a cultura como en-US para o teste
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        var originalUICulture = Thread.CurrentThread.CurrentUICulture;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        try
        {
            var invalidInput = new GetCategoryInput(Guid.Empty);
            var validator = new GetCategoryInputValidator();

            var validationResult = validator.Validate(invalidInput);

            validationResult.Should().NotBeNull();
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
        }
        finally
        {
            // Restaura a cultura original após o teste
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }
    }

}
