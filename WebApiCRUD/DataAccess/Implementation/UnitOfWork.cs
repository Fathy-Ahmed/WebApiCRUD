


using WebApiCRUD.DataAccess.Data;
using WebApiCRUD.Repositories;

namespace  WebApiCRUD.DataAccess.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

     public IProductRepository Product { get; private set; }
     public ICategoryRepository Category { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        this._context = context;
        Product = new ProductRepository(_context);
        Category = new CategoryRepository(_context);
    }

    public async Task<int> Save()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
