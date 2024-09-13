using JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
using JG.Code.Catalog.UnitTests.Application.Genre.Common;

namespace JG.Code.Catalog.UnitTests.Application.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture>
{
}

public class CreateGenreTestFixture : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new CreateGenreInput(GetValidGenreName(), GetRandomBoolean());
}
