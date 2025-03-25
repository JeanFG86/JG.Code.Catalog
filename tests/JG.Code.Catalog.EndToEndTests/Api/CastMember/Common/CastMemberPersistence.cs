using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = JG.Code.Catalog.Domain.Entity;
namespace JG.Code.Catalog.EndToEndTests.Api.CastMember.Common;

public class CastMemberPersistence
{
    private readonly CodeCatalogDbContext _context;

    public CastMemberPersistence(CodeCatalogDbContext context) 
    {
        _context = context;
    }
    
    public async Task InsertList(List<DomainEntity.CastMember> castMembers)
    {
        await _context.AddRangeAsync(castMembers);
        await _context.SaveChangesAsync();
    }
    
    public async Task<DomainEntity.CastMember?> GetById(Guid id) => await _context.CastMembers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    
}