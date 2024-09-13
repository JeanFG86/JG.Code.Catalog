using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.Domain.SeedWork;

namespace JG.Code.Catalog.Domain.Repository;
public interface IGenreRepository : IGenericRepository<Genre>, ISearchableRepository<Genre>
{
}
