using JG.Code.Catalog.Application.Common;
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
        await ValidateAndAddRelations(input, cancellationToken, video);

        try
        {
            await UploadVideoAssets(input, cancellationToken, video);
            await UploadVideoMedias(input, cancellationToken, video);
            await _videoRepository.Insert(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return CreateVideoOutput.FromVideo(video);
        }
        catch (Exception e)
        {
            ClearStorage(cancellationToken, video);
            throw;
        }
    }

    private async Task UploadVideoMedias(CreateVideoInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if (input.Media is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), input.Media.Extension);
            var mediaUrl = await _storageService.Upload(fileName, input.Media.FileStream, cancellationToken);
            video.UpdateMedia(mediaUrl);
        }
        if (input.Trailer is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.Trailer.Extension);
            var mediaUrl = await _storageService.Upload(fileName, input.Trailer.FileStream, cancellationToken);
            video.UpdateTrailer(mediaUrl);
        }
    }

    private void ClearStorage(CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if(video.Thumb is not null)
            _storageService.Delete(video.Thumb.Path, cancellationToken);
        if(video.ThumbHalf is not null)
            _storageService.Delete(video.ThumbHalf.Path, cancellationToken);
        if(video.Banner is not null)
            _storageService.Delete(video.Banner.Path, cancellationToken);
    }

    private async Task UploadVideoAssets(CreateVideoInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if (input.Thumb is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.Thumb.Extension);
            var thumbUrl = await _storageService.Upload(fileName, input.Thumb.FileStream, cancellationToken);
            video.UpdateThumb(thumbUrl);
        }
        if (input.Banner is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.Banner.Extension);
            var bannerUrl = await _storageService.Upload(fileName, input.Banner.FileStream, cancellationToken);
            video.UpdateBanner(bannerUrl);
        }
        if (input.ThumbHalf is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalf.Extension);
            var thumbHalfUrl = await _storageService.Upload(fileName, input.ThumbHalf.FileStream, cancellationToken);
            video.UpdateThumbHalf(thumbHalfUrl);
        }
    }

    private async Task ValidateAndAddRelations(CreateVideoInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
    {
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