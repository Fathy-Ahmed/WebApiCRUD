using WebApiCRUD.DataAccess.Data;
using WebApiCRUD.Models;
using WebApiCRUD.Repositories;

namespace WebApiCRUD.DataAccess.Implementation;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context):base(context)
    {
        this._context = context;
    }
}