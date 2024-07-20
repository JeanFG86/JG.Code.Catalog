using JG.Code.Catalog.Application.Exceptions;
using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.Infra.Data.EF.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly CodeCatalogDbContext _context;
    private DbSet<Category> _categories => _context.Set<Category>();

    public CategoryRepository(CodeCatalogDbContext context)
    {
        _context = context;
    }

    public async Task Insert(Category aggregate, CancellationToken cancellationToken)
    {
        await _categories.AddAsync(aggregate, cancellationToken);
    }

    public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categories.FindAsync(new object[] { id }, cancellationToken);
        NotFoundException.ThrowIfNull(category, $"Category '{id}' not found.");        
        return category!;
    }

    public Task Update(Category aggregate, CancellationToken cancellationToken)
    {
        return Task.FromResult(_categories.Update(aggregate));
    }

    public Task Delete(Category aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }        

    public Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    
}
