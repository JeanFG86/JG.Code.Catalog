using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.Validation;

namespace JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo : ICreateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public CreateVideo(IUnitOfWork unitOfWork,IVideoRepository videoRepository, ICategoryRepository categoryRepository, IGenreRepository genreRepository, ICastMemberRepository castMemberRepository, IStorageService storageService)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
        _castMemberRepository = castMemberRepository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task<CreateVideoOutput> Handle(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var video = new Domain.Entity.Video(input.Title, input.Description, input.YearLaunched, input.Opened, input.Published, input.Duration, input.Rating);
        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors", validationHandler.Errors);
        if ((input.CategoriesIds?.Count ?? 0) > 0)
        {
            await ValidateCategoryIds(input, cancellationToken);
            input.CategoriesIds!.ToList().ForEach(video.AddCategory);
        }
        if ((input.GenresIds?.Count ?? 0) > 0)
        {
            await ValidateAndRetrieveGenreIds(input, cancellationToken);
            input.GenresIds!.ToList().ForEach(video.AddGenre);
        }
        if ((input.CastMembersIds?.Count ?? 0) > 0)
        {
            await ValidateCastMemberIds(input, cancellationToken);
            input.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }
        await _videoRepository.Insert(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return CreateVideoOutput.FromVideo(video);
    }

    private async Task ValidateCastMemberIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _castMemberRepository.GetIdsListByIds(input.CastMembersIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.CastMembersIds!.Count)
        {
            var notFoundIds = input.CastMembersIds!.ToList().FindAll(genreId => !persistenceIds.Contains(genreId));
            throw new RelatedAggregateException($"Related cast member id (or ids) not found: {string.Join(',', notFoundIds)}");
        }
    }

    private async Task ValidateAndRetrieveGenreIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _genreRepository.GetIdsListByIds(input.GenresIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.GenresIds!.Count)
        {
            var notFoundIds = input.GenresIds!.ToList().FindAll(genreId => !persistenceIds.Contains(genreId));
            throw new RelatedAggregateException($"Related genre id (or ids) not found: {string.Join(',', notFoundIds)}");
        }
    }

    private async Task ValidateCategoryIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _categoryRepository.GetIdsListByIds(input.CategoriesIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.CategoriesIds!.Count)
        {
            var notFoundIds = input.CategoriesIds!.ToList().FindAll(categoryId => !persistenceIds.Contains(categoryId));
            throw new RelatedAggregateException($"Related category id (or ids) not found: {string.Join(',', notFoundIds)}");
        }
    }
}