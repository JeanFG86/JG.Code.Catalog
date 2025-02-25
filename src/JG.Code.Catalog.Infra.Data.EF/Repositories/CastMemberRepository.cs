using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.Infra.Data.EF.Repositories;

public class CastMemberRepository : ICastMemberRepository
{
    private readonly CodeCatalogDbContext _context;
    private DbSet<CastMember> _castMembers => _context.Set<CastMember>();

    public CastMemberRepository(CodeCatalogDbContext context)
    {
        _context = context;
    }
    
    public async Task Insert(CastMember aggregate, CancellationToken cancellationToken)
    {
        await _castMembers.AddAsync(aggregate, cancellationToken);
    }

    public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
    {
        var category = await _castMembers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(category, $"CastMember '{id}' not found.");        
        return category!;
    }

    public Task Delete(CastMember aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(CastMember aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}