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
        
        var imagem = new Image(path);

        imagem.Path.Should().Be(path);
    }
}