﻿using JG.Code.Catalog.Application.Exceptions;
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
        var castMember = await _castMembers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(castMember, $"CastMember '{id}' not found.");        
        return castMember!;
    }

    public Task Delete(CastMember aggregate, CancellationToken cancellationToken)
    {
        _castMembers.Remove(aggregate);
        return Task.CompletedTask;
    }

    public Task Update(CastMember aggregate, CancellationToken cancellationToken)
    {
        _castMembers.Update(aggregate);
        return Task.CompletedTask;
    }

    public async Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var items = await _castMembers.AsNoTracking().ToListAsync();
        return new SearchOutput<CastMember>(input.Page, input.PerPage, items.Count, items.AsReadOnly());
    }
}