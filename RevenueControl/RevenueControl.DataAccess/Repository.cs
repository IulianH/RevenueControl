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

        public IList<T> Get(Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "", int take = 0)
        {
            IList<T> returnValue = null;
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if(take > 0)
            {
                query = query.Take(take);
            }

            if (orderBy != null)
            {
                returnValue = orderBy(query).ToList();
            }
            else
            {
                returnValue = query.ToList();
            }
            return returnValue;
        }

        public T GetById(params object[] keys)
        {
            return dbSet.Find(keys);
        }
    }
}
