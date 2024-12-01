using JG.Code.Catalog.Infra.Data.EF;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.Common;

public class GenrePersistence
{
    private readonly CodeCatalogDbContext _context;

    public GenrePersistence(CodeCatalogDbContext context) 
    {
        _context = context;
    }
    
    public async Task InsertList(List<DomainEntity.Genre> genres)
    {
        await _context.AddRangeAsync(genres);
        await _context.SaveChangesAsync();
    }
}