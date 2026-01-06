using JG.Code.Catalog.UnitTests.Common.Fixtures;

namespace JG.Code.Catalog.UnitTests.Application.Video.UpdateVideo;

[CollectionDefinition(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture>
{
}

public class UpdateVideoTestFixture : VideoTestFixtureBase
{
}
