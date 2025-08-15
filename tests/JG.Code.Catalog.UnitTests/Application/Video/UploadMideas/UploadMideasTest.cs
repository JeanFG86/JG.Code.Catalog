namespace JG.Code.Catalog.UnitTests.Application.Video.UploadMideas;

[Collection(nameof(UploadMideasTestFixture))]
public class UploadMideasTest
{
    private readonly UploadMideasTestFixture _fixture;
    
    public UploadMideasTest(UploadMideasTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = nameof(UploadMideas))]
    [Trait("Application ", "UploadMideas - Use Cases")]
    public void UploadMideas()
    {
        
    }
}