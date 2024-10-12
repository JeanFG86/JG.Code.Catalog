
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.Category.ListCategories;
public class ListCategories : IListCategories
{
    private readonly ICategoryRepository _categoryRepository;

    public ListCategories(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ListCategoriesOutput> Handle(ListCategoriesInput input, CancellationToken cancellationToken)
    {
        var searchOutput = await _categoryRepository.Search(input.ToSearchInput(), cancellationToken);
        return ListCategoriesOutput.FromSearchOutput(searchOutput);
    }
}
