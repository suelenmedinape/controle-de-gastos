using backend_dotnet.Data;
using backend_dotnet.Entities;
using backend_dotnet.Interface;

namespace backend_dotnet;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    private IGenericRepository<Person> personRepo;
    public IGenericRepository<Person> PersonRepository
    {
        get { return personRepo ??= new GenericRepository<Person>(_context); }
    }
    
    private IGenericRepository<Categories> categorieRepo;
    public IGenericRepository<Categories> CategoriesRepository
    {
        get { return categorieRepo ??= new GenericRepository<Categories>(_context); }
    }
    
    private IGenericRepository<Transaction> transactionRepo;
    public IGenericRepository<Transaction> TransactionRepository
    {
        get { return transactionRepo ??= new GenericRepository<Transaction>(_context); }
    }
    
    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}