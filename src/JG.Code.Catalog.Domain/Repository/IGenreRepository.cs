using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.Domain.SeedWork;

namespace JG.Code.Catalog.Domain.Repository;
public interface IGenreRepository : IGenericRepository<Genre>, ISearchableRepository<Genre>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken);
    public Task<IReadOnlyList<Genre>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken);
}
