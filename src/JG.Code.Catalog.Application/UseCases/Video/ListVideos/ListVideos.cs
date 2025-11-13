using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.Video.ListVideos;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;

    public ListVideos(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    public async Task<ListVideosOutput> Handle(ListVideosInput input, CancellationToken cancellationToken)
    {
        var searchResult = await _videoRepository.Search(input.ToSearchInput(), cancellationToken);
        return new ListVideosOutput(
            searchResult.CurrentPage,
            searchResult.PerPage,
            searchResult.Total,
            searchResult.Items.Select(VideoModelOutput.FromVideo).ToList()
        );
    }
}
