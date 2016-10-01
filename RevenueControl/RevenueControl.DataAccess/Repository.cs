using System.Data.Entity;
using System.Linq;
using RevenueControl.DomainObjects.Interfaces;

namespace RevenueControl.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly RevenueControlDb context;
        private readonly DbSet<T> dbSet;

        public Repository(RevenueControlDb context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public void Delete(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
                dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Unchanged;
            dbSet.Remove(entity);
        }

        public void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
                dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public T GetById(params object[] keys)
        {
            return dbSet.Find(keys);
        }

        public IQueryable<T> Set
        {
            get { return dbSet; }
        }
    }
}