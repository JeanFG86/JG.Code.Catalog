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
    
    public string GetValidImagePath() => Faker.Image.PlaceImgUrl();
    
    public string GetValidMediaPath()
    {
        var exampleMedias = new[]
        {
            "https://www.googlestorage.com/file-example.mp4",
            "https://www.storage.com/anothe-example.mp4",
            "https://www.googlestorage.com/file-example.mp4",
            "https://www.s3.com.br/example.mp4",
            "https://www.glg.com/file.mp4",
        };
        var random = new Random();
        return exampleMedias[random.Next(exampleMedias.Length)];
    }
    
    public CastMember GetExampleCastMember() => new (GetValidName(), GetRandomCastMemberType());

    public List<CastMember> GetRandomCastMemberList() => Enumerable.Range(0, Random.Shared.Next(1, 5))
        .Select(_ => new CastMember(GetValidName(), GetRandomCastMemberType()))
        .ToList();

    public string GetValidName() => Faker.Name.FullName();
    public CastMemberType GetRandomCastMemberType() => (CastMemberType)(new Random()).Next(1, 2);

    public Category GetExampleCategory() => new(Faker.Commerce.Categories(1)[0], Faker.Commerce.ProductDescription());

    public List<Category> GetRandomCategoryList() => Enumerable.Range(0, Random.Shared.Next(1, 5))
        .Select(_ => GetExampleCategory())
        .ToList();

    public Genre GetExampleGenre() => new(Faker.Commerce.Department());

    public List<Genre> GetRandomGenreList() => Enumerable.Range(0, Random.Shared.Next(1, 5))
        .Select(_ => GetExampleGenre())
        .ToList();
}
