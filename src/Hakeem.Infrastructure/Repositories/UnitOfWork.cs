using Hakeem.Domain.Interfaces;
using Hakeem.Infrastructure.Data;
using System.Collections;

namespace Hakeem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HakeemDbContext _context;
    private Hashtable _repositories;

    public UnitOfWork(HakeemDbContext context)
    {
        _context = context;
        _repositories = new Hashtable();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<T>)_repositories[type]!;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
