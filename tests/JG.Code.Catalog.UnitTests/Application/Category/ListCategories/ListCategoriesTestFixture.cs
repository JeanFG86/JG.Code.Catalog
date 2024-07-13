using JG.Code.Catalog.Application.UseCases.Category.ListCategories;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.UnitTests.Application.Category.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.UnitTests.Application.Category.ListCategories;


[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture> { }

public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
{
    public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10)
    {
        var categories = new List<DomainEntity.Category>();
        for (int i = 0; i < length; i++)
            categories.Add(GetExampleCategory());
        return categories;
    }

    public ListCategoriesInput GetExampleInput()
    {
        var random = new Random();
        return new ListCategoriesInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
            );
    }
}
