﻿using JG.Code.Catalog.Application.UseCases.Category.CreateCategory;
using JG.Code.Catalog.IntegrationTests.Application.UseCases.Category.Common;

namespace JG.Code.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;
[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture>
{
}

public class CreateCategoryTestFixture : CategoryUseCasesBaseFixture
{
    public CreateCategoryInput GetInput()
    {
        var category = GetExampleCategory();
        return new CreateCategoryInput(category.Name, category.Description, category.IsActive);
    }

    public CreateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        return invalidInputShortName;
    }

    public CreateCategoryInput GetInvalidInputTooLongName()
    {
        var invalidInputTooLongName = GetInput();
        var tooLongNameForCategory = Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";

        invalidInputTooLongName.Name = tooLongNameForCategory;

        return invalidInputTooLongName;
    }

    public CreateCategoryInput GetInvalidInputWithDescriptionNull()
    {
        var invalidInputDescriptionNull = GetInput();
        invalidInputDescriptionNull.Description = null;
        return invalidInputDescriptionNull;
    }

    public CreateCategoryInput GetInvalidInputTooLongDescription()
    {
        var invalidInputTooLongDescription = GetInput();
        var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductName()}";
        invalidInputTooLongDescription.Description = tooLongDescriptionForCategory;

        return invalidInputTooLongDescription;
    }
}
