using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.ListVideos;

public class ListVideosInput : PaginatedListInput, IRequest<ListVideosOutput>
{
    public ListVideosInput(int page, int perPage, string search, string sort, SearchOrder dir) : base(page, perPage, search, sort, dir)
    {
    }
}
