using JG.Code.Catalog.Application.UseCases.Category.UpdateCategory;

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
}
