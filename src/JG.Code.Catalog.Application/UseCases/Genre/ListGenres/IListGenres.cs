using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Genre.ListGenres;
public interface IListGenres : IRequestHandler<ListGenresInput, ListGenresOutput>
{
}
