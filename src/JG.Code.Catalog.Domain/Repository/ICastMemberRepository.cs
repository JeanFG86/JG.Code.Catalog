using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.SeedWork;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;

namespace JG.Code.Catalog.Domain.Repository;

public interface ICastMemberRepository : IGenericRepository<CastMember>, ISearchableRepository<CastMember>
{

}