using WebApiCRUD.DataAccess.Data;
using WebApiCRUD.Models;
using WebApiCRUD.Repositories;

namespace WebApiCRUD.DataAccess.Implementation;

public class ProductRepository:GenericRepository<Product>, IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context):base(context)
    {
        this._context = context;
    }
}
