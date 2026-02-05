using JG.Code.Catalog.IntegrationTests.Common;

namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

[CollectionDefinition(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTestFixtureCollection : ICollectionFixture<VideoRepositoryTestFixture>
{

}

public class VideoRepositoryTestFixture : BaseFixture
{

}
