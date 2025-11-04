namespace JG.Code.Catalog.UnitTests.Application.Video.ListVideos;

[Collection(nameof(ListVideosTestFixtureCollection))]
public class ListVideosTest
{
    private readonly ListVideosTestFixture _fixture;

    public ListVideosTest(ListVideosTestFixture fixture)
    {
        _fixture = fixture;
    }
}
