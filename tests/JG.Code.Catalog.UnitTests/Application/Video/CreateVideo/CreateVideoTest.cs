namespace JG.Code.Catalog.UnitTests.Application.Video.CreateVideo;

[Collection(nameof(CreateVideoTestFixtureCollection))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _fixture;
    
    public CreateVideoTest(CreateVideoTestFixture fixture)
    {
        _fixture = fixture;
    }
}