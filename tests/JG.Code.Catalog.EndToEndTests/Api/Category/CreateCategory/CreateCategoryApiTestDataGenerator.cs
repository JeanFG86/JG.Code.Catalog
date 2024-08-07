using JG.Code.Catalog.EndToEndTests.Api.Category.CreateCategory;

namespace JG.Code.Catalog.EndToEndTests.Api.Category;
public class CreateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryApiTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.GetExampleInput();
                    input1.Name = fixture.GetInvalidNameTooShort();
                    invalidInputList.Add(
                        [
                            input1,
                            "Name should be at least 3 characters long"
                        ]);
                    break;
                case 1:
                    var input2 = fixture.GetExampleInput();
                    input2.Name = fixture.GetInvalidNameTooLong();
                    invalidInputList.Add(
                       [
                           input2,
                           "Name should be less or equal 255 characters"
                       ]);
                    break;               
                case 2:
                    var input3 = fixture.GetExampleInput();
                    input3.Description = fixture.GetInvalidDescriptionTooLong();
                    invalidInputList.Add(
                       [
                           input3,
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
