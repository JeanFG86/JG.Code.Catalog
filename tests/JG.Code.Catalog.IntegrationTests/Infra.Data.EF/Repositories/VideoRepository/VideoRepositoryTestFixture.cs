using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.IntegrationTests.Common;

namespace JG.Code.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

[CollectionDefinition(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTestFixtureCollection : ICollectionFixture<VideoRepositoryTestFixture>
{

}

public class VideoRepositoryTestFixture : BaseFixture
{
    public Video GetValidVideo()
    {
        return new Video(GetValidTitle(), GetValidDescription(), GetValidYearLaunched(), GetRandomBoolean(), GetRandomBoolean(), GetValidDuration(), GetRandomRating());
    }

    public string GetValidTitle() => Faker.Lorem.Letter(100);

    public string GetValidDescription() => Faker.Commerce.ProductDescription();

    public int GetValidYearLaunched() => Faker.Random.Int(1950, 2100);

    public int GetValidDuration() => Faker.Random.Int(100, 300);

    public Rating GetRandomRating()
    {
        var values = Enum.GetValues<Rating>();
        var random = new Random();
        return values[random.Next(values.Length)];
    }
}
