using JG.Code.Catalog.Domain.Repository;

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
        var searchOutput = await _repository.Search(input.ToSearchInput(), cancellationToken);
        return ListGenresOutput.FromSearchOutput(searchOutput);
    }
}
