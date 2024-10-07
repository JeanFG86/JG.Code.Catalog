using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Genre.DeleteGenre;
public interface IDeleteGenre : IRequestHandler<DeleteGenreInput>
{
}
