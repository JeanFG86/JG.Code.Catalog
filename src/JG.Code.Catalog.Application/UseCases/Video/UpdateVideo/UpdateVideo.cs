using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.Validation;

namespace JG.Code.Catalog.Application.UseCases.Video.UpdateVideo;

public class UpdateVideo : IUpdateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IStorageService _storageService;

    public UpdateVideo(
        IVideoRepository videoRepository,
        IGenreRepository genreRepository,
        ICategoryRepository categoryRepository,
        ICastMemberRepository castMemberRepository,
        IUnitOfWork unitOfWork,
        IStorageService storageService)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
        _castMemberRepository = castMemberRepository;
        _storageService = storageService;
    }

    public async Task<VideoModelOutput> Handle(UpdateVideoInput request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(request.VideoId, cancellationToken);
        video.Update(
            title: request.Title,
            description: request.Description,
            yearLaunched: request.YearLaunched,
            duration: request.Duration,
            opened: request.Opened,
            published: request.Published,
            rating: request.Rating
        );
        await ValidateAndAddRelations(request, cancellationToken, video);
        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
        {
            throw new EntityValidationException("Could not update video", validationHandler.Errors);
        }

        try
        {
            await UploadVideoAssets(request, cancellationToken, video);
            await UploadVideoMedias(request, cancellationToken, video);
            await _videoRepository.Update(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return VideoModelOutput.FromVideo(video);
        }
        catch (Exception)
        {
            ClearStorage(cancellationToken, video);
            throw;
        }
    }

    private async Task UploadVideoMedias(UpdateVideoInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
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
            var trailerUrl = await _storageService.Upload(fileName, input.Trailer.FileStream, cancellationToken);
            video.UpdateTrailer(trailerUrl);
        }
    }

    private void ClearStorage(CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if (video.Thumb is not null)
            _storageService.Delete(video.Thumb.Path, cancellationToken);
        if (video.ThumbHalf is not null)
            _storageService.Delete(video.ThumbHalf.Path, cancellationToken);
        if (video.Banner is not null)
            _storageService.Delete(video.Banner.Path, cancellationToken);
        if (video.Media is not null)
            _storageService.Delete(video.Media.FilePath, cancellationToken);
        if (video.Trailer is not null)
            _storageService.Delete(video.Trailer.FilePath, cancellationToken);
    }

    private async Task UploadVideoAssets(UpdateVideoInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
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

    private async Task ValidateAndAddRelations(UpdateVideoInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if (input.CategoriesIds is not null)
        {
            video.RemoveAllCategories();
            if (input.CategoriesIds.Any())
            {
                await ValidateCategoryIds(input, cancellationToken);
                input.CategoriesIds!.ToList().ForEach(video.AddCategory);
            }
        }
        if (input.GenresIds is not null)
        {
            video.RemoveAllGenres();
            if (input.GenresIds.Any())
            {
                await ValidateAndRetrieveGenreIds(input, cancellationToken);
                input.GenresIds!.ToList().ForEach(video.AddGenre);
            }
        }
        if (input.CastMembersIds is not null)
        {
            video.RemoveAllCastMembers();

            if (input.CastMembersIds.Any())
            {
                await ValidateCastMemberIds(input, cancellationToken);
                input.CastMembersIds!.ToList().ForEach(video.AddCastMember);

            }
        }
    }

    private async Task ValidateCastMemberIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _castMemberRepository.GetIdsListByIds(input.CastMembersIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.CastMembersIds!.Count)
        {
            var notFoundIds = input.CastMembersIds!.ToList().FindAll(genreId => !persistenceIds.Contains(genreId));
            throw new RelatedAggregateException($"Related cast member id (or ids) not found: {string.Join(',', notFoundIds)}");
        }
    }

    private async Task ValidateAndRetrieveGenreIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _genreRepository.GetIdsListByIds(input.GenresIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.GenresIds!.Count)
        {
            var notFoundIds = input.GenresIds!.ToList().FindAll(genreId => !persistenceIds.Contains(genreId));
            throw new RelatedAggregateException($"Related genre id (or ids) not found: {string.Join(',', notFoundIds)}");
        }
    }

    private async Task ValidateCategoryIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _categoryRepository.GetIdsListByIds(input.CategoriesIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.CategoriesIds!.Count)
        {
            var notFoundIds = input.CategoriesIds!.ToList().FindAll(categoryId => !persistenceIds.Contains(categoryId));
            throw new RelatedAggregateException($"Related category id (or ids) not found: {string.Join(',', notFoundIds)}");
        }
    }
}