using JG.Code.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.UpdateVideo;

public interface IUpdateVideo : IRequestHandler<UpdateVideoInput, VideoModelOutput>
{
}
