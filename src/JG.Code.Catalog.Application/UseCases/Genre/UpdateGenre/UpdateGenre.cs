using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.Genre.UpdateGenre;
public class UpdateGenre : IUpdateGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
    }

    public async Task<GenreModelOutput> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(request.Id, cancellationToken);
        genre.Update(request.Name);
        if(request.IsActive is not null && request.IsActive != genre.IsActive)
        {
            if((bool)request.IsActive)
                genre.Activate();
            else 
                genre.Deactivate();
        }
        await _genreRepository.Update(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return GenreModelOutput.FromGenre(genre);
    }
}
