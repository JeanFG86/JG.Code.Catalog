using JG.Code.Catalog.Application.Common;
using JG.Code.Catalog.Application.Interfaces;
using JG.Code.Catalog.Domain.Repository;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.UploadMideas;

public class UploadMedias : IUploadMedias
{
    private readonly IVideoRepository _videoRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadMedias(IVideoRepository videoRepository, IStorageService storageService, IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UploadMediasInput input, CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(input.VideoId, cancellationToken);
        await UploadVideoFile(input, cancellationToken, video);
        await UploadTrailerFile(input, cancellationToken, video);
        await _videoRepository.Update(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
    }

    private async Task UploadTrailerFile(UploadMediasInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if (input.TrailerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.TrailerFile.Extension);
            var uploadedFilePath = await _storageService.Upload(fileName, input.TrailerFile.FileStream, cancellationToken);
            video.UpdateTrailer(uploadedFilePath);
        }
    }

    private async Task UploadVideoFile(UploadMediasInput input, CancellationToken cancellationToken, Domain.Entity.Video video)
    {
        if (input.VideoFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), input.VideoFile.Extension);
            var uploadedFilePath = await _storageService.Upload(fileName, input.VideoFile.FileStream, cancellationToken);
            video.UpdateMedia(uploadedFilePath);
        }
    }
}