using JG.Code.Catalog.Domain.Repository;

namespace JG.Code.Catalog.Application.UseCases.Video.GetVideo;

public class GetVideo : IGetVideo
{
    private readonly IVideoRepository _videoRepository;

    public GetVideo(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    public async Task<GetVideoOutput> Handle(GetVideoInput request, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(request.VideoId, cancellationToken);
        return GetVideoOutput.FromVideo(video);
    }
}
