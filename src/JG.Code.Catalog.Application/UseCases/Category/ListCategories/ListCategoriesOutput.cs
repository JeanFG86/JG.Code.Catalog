using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Application.UseCases.Category.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;

namespace JG.Code.Catalog.Application.UseCases.Category.ListCategories;
public class ListCategoriesOutput : PaginatedListOutput<CategoryModelOutput>
{
    public ListCategoriesOutput(int page, int perPage, int total, IReadOnlyList<CategoryModelOutput> items) 
        : base(page, perPage, total, items)
    {
    }

    public static ListCategoriesOutput FromSearchOutput(SearchOutput<DomainEntity.Category> searchOutput)
    {
        return new(
           searchOutput.CurrentPage,
           searchOutput.PerPage,
           searchOutput.Total,
           searchOutput.Items.Select(CategoryModelOutput.FromCategory).ToList()
       );
    }
}
