using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Application.UseCases.Video.Common;

namespace JG.Code.Catalog.Application.UseCases.Video.ListVideos;

public class ListVideosOutput : PaginatedListOutput<VideoModelOutput>
{
    public ListVideosOutput(int page, int perPage, int total, IReadOnlyList<VideoModelOutput> items) : base(page, perPage, total, items)
    {
    }
}
