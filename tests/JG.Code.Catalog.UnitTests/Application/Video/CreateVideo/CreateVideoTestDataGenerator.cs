using JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

namespace JG.Code.Catalog.UnitTests.Application.Video.CreateVideo;

public class CreateVideoTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new CreateVideoTestFixture();
        var invalidInputList = new List<object[]>();
        const int totalInvalidCases = 2;

        for (var index = 0; index < times; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add(
                    [
                        new CreateVideoInput(
                        "", 
                        fixture.GetValidDescription(), 
                        fixture.GetRandomBoolean(), 
                        fixture.GetRandomBoolean(),
                        fixture.GetValidYearLaunched(), 
                        fixture.GetValidDuration(), 
                        fixture.GetRandomRating()
                        ),
                        "'Title' is required."
                    ]);
                    break;
                case 1:
                    invalidInputList.Add(
                    [
                        new CreateVideoInput(
                            fixture.GetValidTitle(), 
                            "", 
                            fixture.GetRandomBoolean(), 
                            fixture.GetRandomBoolean(),
                            fixture.GetValidYearLaunched(), 
                            fixture.GetValidDuration(), 
                            fixture.GetRandomRating()
                        ),
                        "'Description' is required."
                    ]);
                    break;
            }
        }
        return invalidInputList;
    }
}