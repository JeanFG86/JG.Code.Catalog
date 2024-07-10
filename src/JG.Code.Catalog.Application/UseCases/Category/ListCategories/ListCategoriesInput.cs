using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Category.ListCategories;
public class ListCategoriesInput : PaginatedListInput, IRequest<ListCategoriesOutput>
{
    public ListCategoriesInput(int page, int perPage, string search, string sort, SearchOrder dir) 
        : base(page, perPage, search, sort, dir)
    {
    }
}
