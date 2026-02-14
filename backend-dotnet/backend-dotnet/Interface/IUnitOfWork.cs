using backend_dotnet.Entities;

namespace backend_dotnet.Interface;

public interface IUnitOfWork
{
    IGenericRepository<Person> PersonRepository { get; }
    IGenericRepository<Categories> CategoriesRepository { get; }
    IGenericRepository<Transaction> TransactionRepository { get; }
    Task CommitAsync();
}