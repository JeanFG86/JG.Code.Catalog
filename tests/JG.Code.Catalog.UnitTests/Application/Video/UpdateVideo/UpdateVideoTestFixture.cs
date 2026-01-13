using JG.Code.Catalog.UnitTests.Common.Fixtures;
using UseCase = JG.Code.Catalog.Application.UseCases.Video.UpdateVideo;

namespace JG.Code.Catalog.UnitTests.Application.Video.UpdateVideo;

[CollectionDefinition(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture>
{
}

public class UpdateVideoTestFixture : VideoTestFixtureBase
{
    public UseCase.UpdateVideoInput CreateValidInput(Guid videoId) => new(
    VideoId: videoId,
    Title: GetValidTitle(),
    Description: GetValidDescription(),
    YearLaunched: GetValidYearLaunched(),
    Duration: GetValidDuration(),
    Opened: GetRandomBoolean(),
    Published: GetRandomBoolean(),
    Rating: GetRandomRating()
);
}
