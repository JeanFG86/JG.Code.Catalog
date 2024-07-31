using JG.Code.Catalog.Application.Interfaces;

namespace JG.Code.Catalog.Infra.Data.EF;
public class UnitOfWork : IUnitOfWork
{
    private readonly CodeCatalogDbContext _context;

    public UnitOfWork(CodeCatalogDbContext context)
    {
        _context = context;
    }

    public Task Commit(CancellationToken cancellationToken)
     => _context.SaveChangesAsync(cancellationToken);
}
