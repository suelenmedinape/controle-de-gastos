namespace backend_dotnet.Interface;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> ListAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}