using JG.Code.Catalog.Infra.Data.EF;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
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
    
    public async Task InsertGenresCategoriesRelationsList(List<GenresCategories> relations)
    {
        await _context.AddRangeAsync(relations);
        await _context.SaveChangesAsync();
    }
    
    public async Task<DomainEntity.Genre?> GetById(Guid id) => await _context.Genres.AsNoTracking().FirstOrDefaultAsync(genre => genre.Id == id);
}