
namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;


[Collection(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTest
{
    private readonly VideoRepositoryTestFixture _fixture;
    public VideoRepositoryTest(VideoRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public void Insert()
    {
    }
}
