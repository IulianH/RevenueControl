using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IRepository<T> 
    {
        void Insert(T entity);
        void Delete(T entity);
        void Update(T entity);
        /*IList<T> Get(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "", int take = 0);*/
        T GetById(params object[] keys);
        IQueryable<T> Set { get; }
    }
}
