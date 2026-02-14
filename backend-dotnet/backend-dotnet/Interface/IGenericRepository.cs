namespace backend_dotnet.Interface;

public interface IGenericRepository<T> where T : class
{
    Task<T?> findById(Guid id);
    Task<List<T>> ListAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    IQueryable<T> Query();
}