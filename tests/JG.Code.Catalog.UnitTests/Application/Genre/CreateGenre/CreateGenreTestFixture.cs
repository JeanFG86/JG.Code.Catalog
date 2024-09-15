﻿using JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
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

    public CreateGenreInput GetExampleInput(string? name)
       => new CreateGenreInput(name!, GetRandomBoolean());


    public CreateGenreInput GetExampleInputWithCategories()
    {
        var numberOfCategoriesIds = (new Random()).Next(1,10);
        var categoriesIds = Enumerable.Range(1, numberOfCategoriesIds).Select(_ => Guid.NewGuid()).ToList();
        return new CreateGenreInput(GetValidGenreName(), GetRandomBoolean(), categoriesIds);
    }
}
