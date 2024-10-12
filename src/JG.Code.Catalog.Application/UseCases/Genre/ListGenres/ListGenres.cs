using JG.Code.Catalog.Application.UseCases.Category.Common;
using JG.Code.Catalog.Application.UseCases.Category.ListCategories;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenres : IListGenres
{
    private readonly IGenreRepository _repository;

    public ListGenres(IGenreRepository repository)
    {
        _repository = repository;
    }

    public async Task<ListGenresOutput> Handle(ListGenresInput input, CancellationToken cancellationToken)
    {
        var searchInput = new SearchInput(input.Page, input.PerPage, input.Search, input.Sort, input.Dir);
        var searchOutput = await _repository.Search(searchInput, cancellationToken);
        return new ListGenresOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items.Select(GenreModelOutput.FromGenre).ToList()
        );
    }
}
