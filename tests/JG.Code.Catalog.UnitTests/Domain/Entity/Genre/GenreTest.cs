using FluentAssertions;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest(GenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggegates")]
    public void Instantiate()
    {
        var genreName = "Horror";

        var datetimeBefore = DateTime.Now;
        var genre = new DomainEntity.Genre(genreName);
        var datetimeAfter = DateTime.Now.AddSeconds(1);
        
        genre.Should().NotBeNull();
        genre.Name.Should().Be(genreName);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        (genre.CreatedAt >= datetimeBefore).Should().BeTrue();
        (genre.CreatedAt <= datetimeAfter).Should().BeTrue();
    }
}
