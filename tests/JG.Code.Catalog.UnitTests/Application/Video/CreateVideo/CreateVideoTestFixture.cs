using JG.Code.Catalog.Application.UseCases.Video.CreateVideo;
using JG.Code.Catalog.UnitTests.Common.Fixtures;

namespace JG.Code.Catalog.UnitTests.Application.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>{}

public class CreateVideoTestFixture : VideoTestFixtureBase
{
    public CreateVideoInput CreateValidVideoInput() =>
        new(
            GetValidTitle(), 
            GetValidDescription(), 
            GetRandomBoolean(), 
            GetRandomBoolean(),
            GetValidYearLaunched(), 
            GetValidDuration(), 
            GetRandomRating()
        );
}