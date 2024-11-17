using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.SeedWork;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;

namespace JG.Code.Catalog.Domain.Repository;
public interface ICategoryRepository : IGenericRepository<Category>, ISearchableRepository<Category>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken);
    public Task<IReadOnlyList<Category>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken);
}
