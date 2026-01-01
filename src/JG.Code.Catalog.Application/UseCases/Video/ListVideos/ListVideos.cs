using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.Video.ListVideos;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ListVideos(IVideoRepository videoRepository, ICategoryRepository categoryRepository)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ListVideosOutput> Handle(ListVideosInput input, CancellationToken cancellationToken)
    {
        var searchResult = await _videoRepository.Search(input.ToSearchInput(), cancellationToken);
        IReadOnlyList<Domain.Entity.Category>? categories = null;
        var relatedCategoryIds = searchResult.Items.SelectMany(video => video.Categories).Distinct().ToList();
        if (relatedCategoryIds.Any())        
            categories = await _categoryRepository.GetListByIds(relatedCategoryIds, cancellationToken);
        
        var output = new ListVideosOutput(
            searchResult.CurrentPage,
            searchResult.PerPage,
            searchResult.Total,
            searchResult.Items.Select(item => VideoModelOutput.FromVideo(item, categories)).ToList()
        );
        return output;
    }
}
