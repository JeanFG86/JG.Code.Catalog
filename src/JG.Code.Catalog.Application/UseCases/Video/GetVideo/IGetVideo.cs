using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.GetVideo;

public interface IGetVideo : IRequestHandler<GetVideoInput, GetVideoOutput>
{
}
