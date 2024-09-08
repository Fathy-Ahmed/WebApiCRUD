using System.Linq.Expressions;

namespace WebApiCRUD.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? Predicate = null, string? IncludeWord = null);
    public Task<T> GetFirstOrDefault(Expression<Func<T, bool>>? Predicate = null, string? IncludeWord = null);
    public Task AddAsync(T entity);
    public void Update(T entity);
    public void Delete(T entity);
    //public Task<int> Count();

    //public Task DeleteRangeAsync(List<T> entities);

}
