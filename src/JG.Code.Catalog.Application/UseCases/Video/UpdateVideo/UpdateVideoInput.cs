using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Enum;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.UpdateVideo;

public record UpdateVideoInput(
    Guid VideoId,
    string Title,
    string Description,
    bool Opened,
    bool Published,
    int YearLaunched,
    int Duration,
    Rating Rating,
    List<Guid>? GenresIds = null,
    List<Guid>? CategoriesIds = null,
    List<Guid>? CastMembersIds = null) : IRequest<VideoModelOutput>;
