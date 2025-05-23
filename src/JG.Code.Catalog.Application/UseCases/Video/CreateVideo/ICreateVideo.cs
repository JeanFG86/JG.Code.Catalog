using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.CreateVideo;

public interface ICreateVideo : IRequestHandler<CreateVideoInput, CreateVideoOutput>
{
    
}