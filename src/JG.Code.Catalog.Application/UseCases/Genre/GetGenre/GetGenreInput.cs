using JG.Code.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Genre.GetGenre;
public class GetGenreInput : IRequest<GenreModelOutput>
{
    public Guid Id { get; set; }

    public GetGenreInput(Guid id) => Id = id;
}
