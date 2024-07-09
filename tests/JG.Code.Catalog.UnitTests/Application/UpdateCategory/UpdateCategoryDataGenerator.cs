using JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;
using JG.Code.Catalog.UnitTests.Application.CreateCategory;

namespace JG.Code.Catalog.UnitTests.Application.UpdateCategory;
public class UpdateCategoryDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();
        for (int i = 0; i < times; i++)
        {
            var exampleCategory = fixture.GetExampleCategory();
            var exampleInput = fixture.GetValidInput(exampleCategory.Id);

            yield return new object[] { exampleCategory, exampleInput };
        }
    }

    public static IEnumerable<Object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new UpdateCategoryTestFixture();
        var invalidInputList = new List<Object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add(
                        [
                            fixture.GetInvalidInputShortName(),
                            "Name should be at least 3 characters long"
                        ]);
                    break;
                case 1:
                    invalidInputList.Add(
                       [
                           fixture.GetInvalidInputTooLongName(),
                                "Name should be less or equal 255 characters"
                       ]);
                    break;               
                case 2:
                    invalidInputList.Add(
                       [
                           fixture.GetInvalidInputTooLongDescription(),
                                "Description should be less or equal 10000 characters"
                       ]);
                    break;
                default:
                    break;
            }
        }
        return invalidInputList;
    }
}
