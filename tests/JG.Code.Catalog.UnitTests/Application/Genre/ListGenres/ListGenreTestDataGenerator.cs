using JG.Code.Catalog.Application.UseCases.Genre.ListGenres;

namespace JG.Code.Catalog.UnitTests.Application.Genre.ListGenres;
public class ListGenreTestDataGenerator
{
    public static IEnumerable<object[]> GetInputsWithoutAllParameter(int times = 14)
    {
        var fixture = new ListGenresTestFixture();
        var inputExample = fixture.GetExampleInput();

        for (int i = 0; i < times; i++)
        {
            switch (i % 7)
            {
                case 0:
                    yield return new object[] { new ListGenresInput() };
                    break;
                case 1:
                    yield return new object[] { new ListGenresInput(inputExample.Page) };
                    break;
                case 2:
                    yield return new object[] { new ListGenresInput(inputExample.Page, inputExample.PerPage) };
                    break;
                case 3:
                    yield return new object[] { new ListGenresInput(inputExample.Page, inputExample.PerPage, inputExample.Search) };
                    break;
                case 4:
                    yield return new object[] { new ListGenresInput(inputExample.Page, inputExample.PerPage, inputExample.Search, inputExample.Sort) };
                    break;
                case 5:
                    yield return new object[] { inputExample };
                    break;
                default:
                    yield return new object[] { new ListGenresInput() };
                    break;
            }
        }
    }
}
