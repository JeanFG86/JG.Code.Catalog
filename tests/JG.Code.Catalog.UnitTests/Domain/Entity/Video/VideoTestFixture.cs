using JG.Code.Catalog.UnitTests.Common;
using JG.Code.Catalog.UnitTests.Common.Fixtures;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.Video;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>{}

public class VideoTestFixture : VideoTestFixtureBase
{
    
}