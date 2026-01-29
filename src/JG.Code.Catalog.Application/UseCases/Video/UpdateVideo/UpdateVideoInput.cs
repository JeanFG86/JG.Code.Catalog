using JG.Code.Catalog.Application.UseCases.Video.Common;
using JG.Code.Catalog.Domain.Enum;
using MediatR;

namespace JG.Code.Catalog.Application.UseCases.Video.UpdateVideo;

public record UpdateVideoInput(
    Guid VideoId,
    string Title,
    string Description,
    int YearLaunched,
    int Duration,
    bool Opened,
    bool Published,
    Rating Rating,
    List<Guid>? CategoriesIds = null,
    List<Guid>? GenresIds = null,
    List<Guid>? CastMembersIds = null,
    FileInput? Thumb = null,
    FileInput? Banner = null,
    FileInput? ThumbHalf = null,
    FileInput? Media = null,
    FileInput? Trailer = null
) : IRequest<VideoModelOutput>;