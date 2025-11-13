using JG.Code.Catalog.UnitTests.Common.Fixtures;
using Entities = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Application.Video.ListVideos;

[CollectionDefinition(nameof(ListVideosTestFixtureCollection))]
public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture>
{
}

public class ListVideosTestFixture : VideoTestFixtureBase
{
    public List<Entities.Video> CreateExampleVideosList() => Enumerable.Range(1, Random.Shared.Next(10, 20)).Select(_ => GetValidVideoWithAllProperties()).ToList();
}
