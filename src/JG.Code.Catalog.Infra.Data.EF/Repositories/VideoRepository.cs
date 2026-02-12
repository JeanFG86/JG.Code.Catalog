using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.Infra.Data.EF.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly CodeCatalogDbContext _context;
    private DbSet<Video> _videos => _context.Set<Video>();

    public VideoRepository(CodeCatalogDbContext context)
    {
        _context = context;
    }

    public async Task Insert(Video aggregate, CancellationToken cancellationToken)
    {
        await _videos.AddAsync(aggregate, cancellationToken);
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
        throw new NotImplementedException();
    }
}
