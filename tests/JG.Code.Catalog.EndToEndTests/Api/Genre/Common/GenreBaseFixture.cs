using JG.Code.Catalog.EndToEndTests.Common;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.Common;

public class GenreBaseFixture : BaseFixture
{
    public GenrePersistence Persistence { get; set; }

    public GenreBaseFixture() : base()
    {
        Persistence = new GenrePersistence(CreateDbContext());
    }
}