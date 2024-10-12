using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;

namespace JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenresOutput : PaginatedListOutput<GenreModelOutput>
{
    public ListGenresOutput(int page, int perPage, int total, IReadOnlyList<GenreModelOutput> items) 
        : base(page, perPage, total, items)
    {
    }

    public static ListGenresOutput FromSearchOutput(SearchOutput<DomainEntity.Genre> searchOutput)
    {
        return new(
           searchOutput.CurrentPage,
           searchOutput.PerPage,
           searchOutput.Total,
           searchOutput.Items.Select(GenreModelOutput.FromGenre).ToList()
       );
    }
}
