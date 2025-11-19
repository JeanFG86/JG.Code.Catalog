using JG.Code.Catalog.Domain.Enum;
using JG.Code.Catalog.Domain.Extensions;

namespace JG.Code.Catalog.Application.UseCases.Video.Common;

public record VideoModelOutput(
    Guid Id,
    DateTime CreatedAt,
    string Title,
    bool Published,
    string Description,
    string Rating,
    int YearLaunched,
    int Duration,
    bool Opened,
    IReadOnlyCollection<Guid> CategoriesIds,
    IReadOnlyCollection<Guid> GenresIds,
    IReadOnlyCollection<Guid> CastMembersIds,
    IReadOnlyCollection<RelatedAggregate> Categories,
    IReadOnlyCollection<RelatedAggregate> Genres,
    IReadOnlyCollection<RelatedAggregate> CastMembers,
    string? ThumbFileUrl = null,
    string? BannerFileUrl = null,
    string? ThumbHalfFileUrl = null,
    string? VideoFileUrl = null,
    string? TrailerFileUrl = null)
{
    public static VideoModelOutput FromVideo(Domain.Entity.Video video)
    {
        return new VideoModelOutput(video.Id, video.CreatedAt, video.Title, video.Published, video.Description, video.Rating.ToStringSignal(),
            video.YearLaunched, video.Duration, video.Opened, video.Categories, video.Genres, video.CastMembers,
            video.Categories.Select(id => new RelatedAggregate(id)).ToList(), video.Genres.Select(id => new RelatedAggregate(id)).ToList(), video.CastMembers.Select(id => new RelatedAggregate(id)).ToList(), 
            video.Thumb?.Path, video.Banner?.Path, video.ThumbHalf?.Path, video.Media?.FilePath, video.Trailer?.FilePath);
    }
}


public record RelatedAggregate(Guid Id, string? Name = null);