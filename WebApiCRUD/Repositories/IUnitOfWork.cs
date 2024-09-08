namespace WebApiCRUD.Repositories;

public interface IUnitOfWork:IDisposable
{
    IProductRepository Product { get; }
    ICategoryRepository Category { get; }

    Task<int> Save();
}
