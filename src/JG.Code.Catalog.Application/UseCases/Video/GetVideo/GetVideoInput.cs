using JG.Code.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.GetVideo;

public record GetVideoInput(Guid VideoId): IRequest<VideoModelOutput>;
