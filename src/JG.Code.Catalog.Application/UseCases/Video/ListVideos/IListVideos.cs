using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.ListVideos;

public interface IListVideos : IRequestHandler<ListVideosInput, ListVideosOutput>
{
}
