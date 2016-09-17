using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects;
using System.Linq.Expressions;
using System.Data.Entity;

namespace RevenueControl.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        RevenueControlDb context;
        DbSet<T> dbSet;
        public Repository(RevenueControlDb context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public void Delete(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            context.Entry(entity).State = EntityState.Unchanged;
            context.Set<T>().Remove(entity);
        }

        public void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            context.Entry(entity).State = EntityState.Modified; 
        }

        public IEnumerable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Where(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet;
        }

        public T GetById(params object[] keys)
        {
            return dbSet.Find(keys);
        }
    }
}
