using JG.Code.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Genre.UpdateGenre;
public interface IUpdateGenre : IRequestHandler<UpdateGenreInput, GenreModelOutput>
{
}
