﻿using JG.Code.Catalog.Application.Exceptions;
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
        var category = await _categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(category, $"Category '{id}' not found.");        
        return category!;
    }

    public Task Update(Category aggregate, CancellationToken _)
    {
        return Task.FromResult(_categories.Update(aggregate));
    }

    public Task Delete(Category aggregate, CancellationToken _)
    {
        return Task.FromResult(_categories.Remove(aggregate));
    }        

    public async Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var total = await _categories.CountAsync();
        var items = await _categories.AsNoTracking().Skip(toSkip).Take(input.PerPage).ToListAsync();
        return new SearchOutput<Category>(input.Page, input.PerPage, total, items);
    }

    
}
