using JG.Code.Catalog.UnitTests.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.Video;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>{}

public class VideoTestFixture : BaseFixture
{
    public DomainEntity.Video GetValidVideo()
    {
        return new DomainEntity.Video("Title", "Description", true, true, 2001, 180);
    }
}