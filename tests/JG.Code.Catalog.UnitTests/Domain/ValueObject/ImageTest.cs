using FluentAssertions;
using JG.Code.Catalog.Domain.ValueObject;
using JG.Code.Catalog.UnitTests.Common;

namespace JG.Code.Catalog.UnitTests.Domain.ValueObject;

public class ImageTest : BaseFixture
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Image - ValueObjects")]
    public void Instantiate()
    {
        var path = Faker.Image.PicsumUrl();
        
        var image = new Image(path);

        image.Path.Should().Be(path);
    }
    
    [Fact(DisplayName = nameof(EqualsByPath))]
    [Trait("Domain", "Image - ValueObjects")]
    public void EqualsByPath()
    {
        var path = Faker.Image.PicsumUrl();
        var image = new Image(path);
        var sameImage = new Image(path);

        var isItEquals = image == sameImage;

        isItEquals.Should().BeTrue();
    }
}