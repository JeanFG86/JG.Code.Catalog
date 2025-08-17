using System.Text;
using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Enum;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Common.Fixtures;

public abstract class VideoTestFixtureBase : BaseFixture
{
    public DomainEntity.Video GetValidVideo()
    {
        return new DomainEntity.Video(GetValidTitle(), GetValidDescription(), GetValidYearLaunched(), GetRandomBoolean(), GetRandomBoolean(), GetValidDuration(), GetRandomRating());
    }

    public Rating GetRandomRating()
    {
        var values = Enum.GetValues<Rating>();
        var random = new Random();
        return values[random.Next(values.Length)];
    }

    public string GetValidTitle() => Faker.Lorem.Letter(100);

    public string GetValidDescription() => Faker.Commerce.ProductDescription();

    public int GetValidYearLaunched() => Faker.Random.Int(1950, 2100);

    public int GetValidDuration() => Faker.Random.Int(100, 300);

    public string GetTooLongTitle() => Faker.Lorem.Letter(400);
    
    public string GetTooLongDescription() => Faker.Lorem.Letter(4001);
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

    public DomainEntity.Media GetValidMedia() => new (GetValidMediaPath());
    
    public FileInput GetValidImageFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
        var fileInput = new FileInput("jpg", exampleStream);
        return fileInput;
    }
    
    public FileInput GetValidMediaFileInput()
    {
        var exampleStream = new MemoryStream(Encoding.ASCII.GetBytes("test"));
        var fileInput = new FileInput("mp4", exampleStream);
        return fileInput;
    }
}