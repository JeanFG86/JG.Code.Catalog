using JG.Code.Catalog.Domain.Entity;
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

    public Task Delete(Video aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Video> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }    

    public Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Video aggregate, CancellationToken cancellationToken)
    {
        _videos.Update(aggregate);
        return Task.CompletedTask;   
    }
}
