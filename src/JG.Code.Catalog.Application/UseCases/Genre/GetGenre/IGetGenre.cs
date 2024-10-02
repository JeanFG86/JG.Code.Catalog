using JG.Code.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Genre.GetGenre;
public interface IGetGenre : IRequestHandler<GetGenreInput, GenreModelOutput>
{
}
