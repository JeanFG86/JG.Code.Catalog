namespace JG.Code.Catalog.UnitTests.Application.Video.GetVideo;

[Collection(nameof(GetVideoTestFixture))]
public class GetVideoTest
{
    private readonly GetVideoTestFixture _fixture;
    
    public GetVideoTest(GetVideoTestFixture fixture)
    {
        _fixture = fixture;
    }
}