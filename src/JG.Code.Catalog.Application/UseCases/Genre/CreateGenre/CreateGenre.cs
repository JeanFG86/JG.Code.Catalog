using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.Genre.Common;
using JG.Code.Catalog.Domain.Repository;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.Application.UseCases.Genre.CreateGenre;
public class CreateGenre : ICreateGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var genre = new DomainEntity.Genre(input.Name, input.IsActive);
        if (input.CategoriesIds is not null)
        {
            await ValidateCategoriesIds(input, cancellationToken);
            input.CategoriesIds.ForEach(genre.AddCategory);
        }
        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return GenreModelOutput.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var idsInPersistence = await _categoryRepository.GetIdsListByIds(input.CategoriesIds!, cancellationToken);
        if (idsInPersistence.Count < input.CategoriesIds!.Count)
        {
            var notFoundIds = input.CategoriesIds.FindAll(x => !idsInPersistence.Contains(x));
            var notFoundIdsAsString = String.Join(", ", notFoundIds);
            throw new RelatedAggregateException($"Related category id (or ids) not found: {notFoundIdsAsString}");
        }
    }
}
