using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Domain.Repository;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
public class CreateGenre : ICreateGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _genreRepository = genreRepository;
    }

    public async Task<GenreModelOutput> Handle(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var genre = new DomainEntity.Genre(input.Name, input.IsActive);
        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return new GenreModelOutput(genre.Id, genre.Name, genre.IsActive, genre.CreatedAt, genre.Categories);
    }
}
