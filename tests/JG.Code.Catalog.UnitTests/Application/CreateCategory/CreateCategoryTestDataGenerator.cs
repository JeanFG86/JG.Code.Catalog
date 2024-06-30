namespace JG.Code.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<Object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputList = new List<Object[]>();
        var totalInvalidCases = 4;

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
                               fixture.GetInvalidInputWithDescriptionNull(),
                                "Description should not be null"
                           ]);
                    break;
                    case 3:
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
