using JG.Code.Catalog.UnitTests.Application.Genre.Common;

namespace JG.Code.Catalog.UnitTests.Application.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureeCollection : ICollectionFixture<GetGenreTestFixture>
{
}

public class GetGenreTestFixture : GenreUseCasesBaseFixture
{
}
