using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.Video.DeleteVideo;

public class DeleteVideo : IDeleteVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteVideoInput input, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(input.Id, cancellationToken);
        await _videoRepository.Delete(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
    }
}
