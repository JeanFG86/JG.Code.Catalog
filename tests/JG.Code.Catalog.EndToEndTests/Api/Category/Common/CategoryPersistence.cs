using JG.Code.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = JG.Code.Catalog.Domain.Entity;

namespace JG.Code.Catalog.EndToEndTests.Api.Category.Common;
public class CategoryPersistence
{
    private readonly CodeCatalogDbContext _context;

    public CategoryPersistence(CodeCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<DomainEntity.Category?> GetById(Guid id)
     => await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task InsertList(List<DomainEntity.Category> categories)
    {
        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }
}
