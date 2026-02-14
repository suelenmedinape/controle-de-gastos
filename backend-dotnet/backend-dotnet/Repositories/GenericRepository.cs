using backend_dotnet.Data;
using backend_dotnet.Interface;
using Microsoft.EntityFrameworkCore;

namespace backend_dotnet;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    public readonly AppDbContext _context;
    
    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<T?> findById(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    
    public async Task<List<T>> ListAll()
    {
        return await _context.Set<T>().ToListAsync();
    }
    
    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }
    
    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
    
    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
    
    public IQueryable<T> Query()
    {
        return _context.Set<T>().AsQueryable();
    }
}