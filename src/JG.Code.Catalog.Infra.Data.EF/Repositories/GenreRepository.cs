using JG.Code.Catalog.Domain.Entity;
using JG.Code.Catalog.Domain.Repository;
using JG.Code.Catalog.Domain.SeedWork.SearchableRepository;
using JG.Code.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace JG.Code.Catalog.Infra.Data.EF.Repositories;
public class GenreRepository : IGenreRepository
{
    private readonly CodeCatalogDbContext _context;
    private DbSet<Genre> _genres => _context.Set<Genre>();
    private DbSet<GenresCategories> _genresCategories => _context.Set<GenresCategories>();

    public GenreRepository(CodeCatalogDbContext context)
    {
        _context = context;
    }
    public async Task Insert(Genre genre, CancellationToken cancellationToken)
    {
        await _genres.AddAsync(genre);
        if (genre.Categories.Any()) 
        {
            var relations = genre.Categories.Select(categoryId => new GenresCategories(categoryId, genre.Id));
            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Genre> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }    

    public Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
