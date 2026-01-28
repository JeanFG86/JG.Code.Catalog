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

    public UpdateVideo(IVideoRepository videoRepository, IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _genreRepository = genreRepository;
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
        if(validationHandler.HasErrors())
        {
            throw new EntityValidationException("Could not update video", validationHandler.Errors);
        }
        await _videoRepository.Update(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }

    private async Task ValidateAndAddRelations(UpdateVideoInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if ((input.GenresIds?.Count ?? 0) > 0)
        {
            await ValidateAndRetrieveGenreIds(input, cancellationToken);
            input.GenresIds!.ToList().ForEach(video.AddGenre);
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
}
