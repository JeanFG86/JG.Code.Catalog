using JG.Code.Catalog.Application.Exceptions;
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

    public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
    {
        var genre = await _genres.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(genre, $"Genre {id} not found.");
        var categoryIds = await _genresCategories.Where(x => x.GenreId == genre.Id).Select(x => x.CategoryId).ToListAsync(cancellationToken);
        categoryIds.ForEach(genre.AddCategory);
        return genre;
    }

    public Task Delete(Genre genre, CancellationToken cancellationToken)
    {
        _genresCategories.RemoveRange(_genresCategories.Where(x => x.GenreId == genre.Id));
        return Task.FromResult(_genres.Remove(genre));
    }

    public async Task Update(Genre genre, CancellationToken cancellationToken)
    {
        _genres.Update(genre);
        _genresCategories.RemoveRange(_genresCategories.Where(x => x.GenreId == genre.Id));
        if (genre.Categories.Any())
        {
            var relations = genre.Categories.Select(categoryId => new GenresCategories(categoryId, genre.Id));
            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var genres = await _genres.Skip(toSkip).Take(input.PerPage).ToListAsync();
        var total = await _genres.CountAsync();
        var genresIds = genres.Select(genre => genre.Id).ToList();
        var relations = await _genresCategories.Where(relation => genresIds.Contains(relation.GenreId)).ToListAsync();
        var relationByGenreIdGroup = relations.GroupBy(x => x.GenreId).ToList();
        relationByGenreIdGroup.ForEach(relationGroup =>
        {
            var genre = genres.Find(genre => genre.Id == relationGroup.Key);
            if(genre is null) return;
            relationGroup.ToList().ForEach(relation => genre.AddCategory(relation.CategoryId));
        });
        return new SearchOutput<Genre>(input.Page, input.PerPage, total, genres);
    }    
}
