
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApiCRUD.DataAccess.Data;
using WebApiCRUD.Repositories;

namespace WebApiCRUD.DataAccess.Implementation;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> dbSet;
    public GenericRepository
        (AppDbContext context)
    {
        _context = context;
        dbSet = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await dbSet.AddAsync(entity);
    }
    public void Delete(T entity)
    {
        dbSet.Remove(entity);
    }
    public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? Predicate, string? IncludeWord)
    {
        IQueryable<T> query = dbSet;
        if (Predicate != null)
        {
            query = query.Where(Predicate);
        }
        if (IncludeWord != null)
        {
            foreach (var item in IncludeWord.Split(',',StringSplitOptions.RemoveEmptyEntries))
            {
                query=query.Include(item);
            }
        }
        return (await query.ToListAsync());
    }
    public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>>? Predicate, string? IncludeWord)
    {
        IQueryable<T> query = dbSet;
        if (Predicate != null)
        {
            query = query.Where(Predicate);
        }
        if (IncludeWord != null)
        {
            foreach (var item in IncludeWord.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(item);
            }
        }
        return (await query.SingleOrDefaultAsync());
    }
    public void Update(T entity)
    {
         dbSet.Update(entity);
    }


    //public async Task<int> Count()
    //{
    //    return await dbSet.CountAsync();
    //}


}
