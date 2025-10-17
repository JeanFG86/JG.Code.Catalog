using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.GetVideo;

public record GetVideoInput(Guid VideoId): IRequest<GetVideoOutput>;
