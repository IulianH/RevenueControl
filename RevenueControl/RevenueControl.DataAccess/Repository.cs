using System.Data.Entity;
using System.Linq;
using RevenueControl.DomainObjects.Interfaces;

namespace RevenueControl.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly RevenueControlDb _context;
        private readonly DbSet<T> _dbSet;

        public Repository(RevenueControlDb context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Unchanged;
            _dbSet.Remove(entity);
        }

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public T GetById(params object[] keys)
        {
            return _dbSet.Find(keys);
        }

        public IQueryable<T> Set => _dbSet;
    }
}