using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.SeedWork;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;

namespace JG.Code.Catalog.Domain.Repository;

public interface ICastMemberRepository : IGenericRepository<CastMember>, ISearchableRepository<CastMember>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken);
}