using JG.Code.Catalog.Domain.Repository;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenres : IListGenres
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ListGenres(IGenreRepository repository, ICategoryRepository categoryRepository)
    {
        _genreRepository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ListGenresOutput> Handle(ListGenresInput input, CancellationToken cancellationToken)
    {
        var searchOutput = await _genreRepository.Search(input.ToSearchInput(), cancellationToken);
        ListGenresOutput output = ListGenresOutput.FromSearchOutput(searchOutput);
        List<Guid> relatedCategoriesIds = searchOutput.Items.SelectMany(item => item.Categories).Distinct().ToList();
        if (relatedCategoriesIds.Count > 0)
        {
            IReadOnlyList<DomainEntity.Category> categories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
            output.FillWithCategoryNames(categories);
        }
        return output;
    }
}
