using Bogus;
using FluentAssertions;
using JG.Code.Catalog.Domain.Validation;


namespace JG.Code.Catalog.UnitTests.Domain.Validation;
public class DomainValidationTest
{
    private Faker Faker {  get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName;
        Action action = () => DomainValidation.NotNull(value, "Value");
        action.Should().NotThrow();
    }
}
