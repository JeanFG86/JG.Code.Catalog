using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.Infra.Data.EF.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly CodeCatalogDbContext _context;
    private DbSet<Video> _videos => _context.Set<Video>();
    private DbSet<VideosCategories> _videosCategories => _context.Set<VideosCategories>();
    private DbSet<VideosGenres> _videosGenres => _context.Set<VideosGenres>();
    private DbSet<VideosCastMembers> _videosCastMembers => _context.Set<VideosCastMembers>();

    public VideoRepository(CodeCatalogDbContext context)
    {
        _context = context;
    }

    public async Task Insert(Video video, CancellationToken cancellationToken)
    {
        await _videos.AddAsync(video, cancellationToken);
        if (video.Categories.Any()) 
        {
            var relations = video.Categories.Select(categoryId => new VideosCategories(categoryId, video.Id));
            await _videosCategories.AddRangeAsync(relations);
        }
        if (video.Genres.Any())
        {
            var relations = video.Genres.Select(genreId => new VideosGenres(genreId, video.Id));
            await _videosGenres.AddRangeAsync(relations);
        }
        if (video.CastMembers.Any())
        {
            var relations = video.CastMembers.Select(castMemberId => new VideosCastMembers(castMemberId, video.Id));
            await _videosCastMembers.AddRangeAsync(relations);
        }
    }

    public async Task Delete(Video aggregate, CancellationToken cancellationToken)
    {
        var categoryRelations = await _videosCategories
            .Where(r => r.VideoId == aggregate.Id)
            .ToListAsync(cancellationToken);
        _videosCategories.RemoveRange(categoryRelations);

        var genreRelations = await _videosGenres
            .Where(r => r.VideoId == aggregate.Id)
            .ToListAsync(cancellationToken);
        _videosGenres.RemoveRange(genreRelations);

        var castMemberRelations = await _videosCastMembers
            .Where(r => r.VideoId == aggregate.Id)
            .ToListAsync(cancellationToken);
        _videosCastMembers.RemoveRange(castMemberRelations);

        _videos.Remove(aggregate);
    }

    public async Task<Video> Get(Guid id, CancellationToken cancellationToken)
    {
        var video = await _videos
            .AsNoTracking()
            .FirstOrDefaultAsync(video => video.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(video, $"Video '{id}' not found.");

        await AddRelationsToVideos([video!], cancellationToken);
        return video!;
    }    

    public async Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var skip = (input.Page - 1) * input.PerPage;
        var query = _videos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(video => video.Title.Contains(input.Search));

        var total = await query.CountAsync(cancellationToken);
        var videos = await AddOrderToQuery(query, input.OrderBy, input.Order)
            .Skip(skip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        await AddRelationsToVideos(videos, cancellationToken);
        return new SearchOutput<Video>(input.Page, input.PerPage, total, videos);
    }

    public async Task Update(Video aggregate, CancellationToken cancellationToken)
    {
        var persistedVideo = await _videos
            .AsNoTracking()
            .FirstOrDefaultAsync(video => video.Id == aggregate.Id, cancellationToken);
        _videos.Update(aggregate);

        MarkOwnedAsAddedWhenNew(persistedVideo?.Thumb, aggregate.Thumb);
        MarkOwnedAsAddedWhenNew(persistedVideo?.ThumbHalf, aggregate.ThumbHalf);
        MarkOwnedAsAddedWhenNew(persistedVideo?.Banner, aggregate.Banner);
        MarkOwnedAsAddedWhenNew(persistedVideo?.Media, aggregate.Media);
        MarkOwnedAsAddedWhenNew(persistedVideo?.Trailer, aggregate.Trailer);

        _videosCategories.RemoveRange(_videosCategories.Where(relation => relation.VideoId == aggregate.Id));
        _videosGenres.RemoveRange(_videosGenres.Where(relation => relation.VideoId == aggregate.Id));
        _videosCastMembers.RemoveRange(_videosCastMembers.Where(relation => relation.VideoId == aggregate.Id));

        await _videosCategories.AddRangeAsync(
            aggregate.Categories.Select(categoryId => new VideosCategories(categoryId, aggregate.Id)),
            cancellationToken);
        await _videosGenres.AddRangeAsync(
            aggregate.Genres.Select(genreId => new VideosGenres(genreId, aggregate.Id)),
            cancellationToken);
        await _videosCastMembers.AddRangeAsync(
            aggregate.CastMembers.Select(castMemberId => new VideosCastMembers(castMemberId, aggregate.Id)),
            cancellationToken);
    }

    private void MarkOwnedAsAddedWhenNew(object? persistedValue, object? currentValue)
    {
        if (persistedValue is null && currentValue is not null)
            _context.Entry(currentValue).State = EntityState.Added;
    }

    private static IQueryable<Video> AddOrderToQuery(
        IQueryable<Video> query,
        string orderProperty,
        SearchOrder order)
    {
        return (orderProperty.ToLower(), order) switch
        {
            ("title", SearchOrder.Asc) => query.OrderBy(video => video.Title).ThenBy(video => video.Id),
            ("title", SearchOrder.Desc) => query.OrderByDescending(video => video.Title).ThenByDescending(video => video.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(video => video.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(video => video.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(video => video.CreatedAt).ThenBy(video => video.Id),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(video => video.CreatedAt).ThenByDescending(video => video.Id),
            _ => query.OrderBy(video => video.Title).ThenBy(video => video.Id)
        };
    }

    private async Task AddRelationsToVideos(List<Video> videos, CancellationToken cancellationToken)
    {
        if (videos.Count == 0)
            return;

        var videoById = videos.ToDictionary(video => video.Id);
        var videoIds = videoById.Keys.ToList();

        var categoryRelations = await _videosCategories
            .AsNoTracking()
            .Where(relation => videoIds.Contains(relation.VideoId))
            .ToListAsync(cancellationToken);
        categoryRelations.ForEach(relation => videoById[relation.VideoId].AddCategory(relation.CategoryId));

        var genreRelations = await _videosGenres
            .AsNoTracking()
            .Where(relation => videoIds.Contains(relation.VideoId))
            .ToListAsync(cancellationToken);
        genreRelations.ForEach(relation => videoById[relation.VideoId].AddGenre(relation.GenreId));

        var castMemberRelations = await _videosCastMembers
            .AsNoTracking()
            .Where(relation => videoIds.Contains(relation.VideoId))
            .ToListAsync(cancellationToken);
        castMemberRelations.ForEach(relation => videoById[relation.VideoId].AddCastMember(relation.CastMemberId));
    }
}
