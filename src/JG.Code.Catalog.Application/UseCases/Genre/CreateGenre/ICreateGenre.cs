using JG.Code.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
public interface ICreateGenre : IRequestHandler<CreateGenreInput, GenreModelOutput>
{
}
