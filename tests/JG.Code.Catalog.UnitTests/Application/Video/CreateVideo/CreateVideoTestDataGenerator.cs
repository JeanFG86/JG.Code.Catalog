using System.Collections;
using JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

namespace JG.Code.Catalog.UnitTests.Application.Video.CreateVideo;

public class CreateVideoTestDataGenerator : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var fixture = new CreateVideoTestFixture();
        var invalidInputList = new List<object[]>();
        const int totalInvalidCases = 4;

        for (var index = 0; index < totalInvalidCases * 2; index++)
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
                case 2:
                    invalidInputList.Add(
                    [
                        new CreateVideoInput(
                            fixture.GetTooLongTitle(), 
                            fixture.GetValidDescription(), 
                            fixture.GetRandomBoolean(), 
                            fixture.GetRandomBoolean(),
                            fixture.GetValidYearLaunched(), 
                            fixture.GetValidDuration(), 
                            fixture.GetRandomRating()
                        ),
                        "'Title' should be less or equal 255 characters long."
                    ]);
                    break;
                case 3:
                    invalidInputList.Add(
                    [
                        new CreateVideoInput(
                            fixture.GetValidTitle(), 
                            fixture.GetTooLongDescription(), 
                            fixture.GetRandomBoolean(), 
                            fixture.GetRandomBoolean(),
                            fixture.GetValidYearLaunched(), 
                            fixture.GetValidDuration(), 
                            fixture.GetRandomRating()
                        ),
                        "'Description' should be less or equal 4000 characters long."
                    ]);
                    break;
                default:
                    break;
            }
        }
        return invalidInputList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}