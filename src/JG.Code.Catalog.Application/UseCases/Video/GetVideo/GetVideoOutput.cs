using JG.Code.Catalog.Domain.Enum;

namespace JG.Code.Catalog.Application.UseCases.Video.GetVideo;

public record GetVideoOutput(
    Guid Id,
    DateTime CreatedAt,
    string Title,
    bool Published,
    string Description,
    Rating Rating,
    int YearLaunched,
    int Duration,
    bool Opened,
    IReadOnlyCollection<Guid> CategoriesIds,
    IReadOnlyCollection<Guid> GenresIds,
    IReadOnlyCollection<Guid> CastMembersIds,
    string? Thumb = null,
    string? Banner = null,
    string? ThumbHalf = null,
    string? Media = null,
    string? Trailer = null)
{
    public static GetVideoOutput FromVideo(Domain.Entity.Video video)
    {
        return new GetVideoOutput(video.Id, video.CreatedAt, video.Title, video.Published, video.Description, video.Rating,
            video.YearLaunched, video.Duration, video.Opened, video.Categories, video.Genres, video.CastMembers, video.Thumb?.Path,
            video.Banner?.Path, video.ThumbHalf?.Path, video.Media?.FilePath, video.Trailer?.FilePath);
    }
}
