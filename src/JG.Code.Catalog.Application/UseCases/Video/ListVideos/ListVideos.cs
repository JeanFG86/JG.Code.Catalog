using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.Video.ListVideos;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;

    public ListVideos(IVideoRepository videoRepository, ICategoryRepository categoryRepository, IGenreRepository genreRepository)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
    }

    public async Task<ListVideosOutput> Handle(ListVideosInput input, CancellationToken cancellationToken)
    {
        var searchResult = await _videoRepository.Search(input.ToSearchInput(), cancellationToken);
        IReadOnlyList<Domain.Entity.Category>? categories = null;
        var relatedCategoryIds = searchResult.Items.SelectMany(video => video.Categories).Distinct().ToList();
        if (relatedCategoryIds.Any())        
            categories = await _categoryRepository.GetListByIds(relatedCategoryIds, cancellationToken);
        IReadOnlyList<Domain.Entity.Genre>? genres = null;
        var relatedGenreIds = searchResult.Items.SelectMany(video => video.Genres).Distinct().ToList();
        if (relatedGenreIds.Any())
            genres =  await _genreRepository.GetListByIds(relatedGenreIds, cancellationToken);
        var output = new ListVideosOutput(
            searchResult.CurrentPage,
            searchResult.PerPage,
            searchResult.Total,
            searchResult.Items.Select(item => VideoModelOutput.FromVideo(item, categories, genres)).ToList()
        );
        return output;
    }
}
