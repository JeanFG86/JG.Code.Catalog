namespace JG.Code.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTestDataGenerator
{
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
