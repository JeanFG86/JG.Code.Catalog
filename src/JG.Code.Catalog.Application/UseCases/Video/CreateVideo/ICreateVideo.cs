using JG.Code.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

public interface ICreateVideo : IRequestHandler<CreateVideoInput, VideoModelOutput>
{
    
}