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
    public UpdateVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
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
}
