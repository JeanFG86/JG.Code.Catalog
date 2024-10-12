using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenresInput : PaginatedListInput, IRequest<ListGenresOutput>
{
    public ListGenresInput()
        : base(1, 15, "", "", SearchOrder.Asc)
    {
    }

    public ListGenresInput(int page = 1, int perPage = 15, string search = "", string sort = "", SearchOrder dir = SearchOrder.Asc)
        : base(page, perPage, search, sort, dir)
    {
    }
}
