using JG.Code.Catalog.Infra.Data.EF;

namespace JG.Code.Catalog.EndToEndTests.Api.Genre.Common;

public class GenrePersistence
{
    private readonly CodeCatalogDbContext _context;

    public GenrePersistence(CodeCatalogDbContext context)
    {
        _context = context;
    }
}