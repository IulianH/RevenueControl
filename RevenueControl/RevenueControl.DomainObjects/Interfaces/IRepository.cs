using System.Linq;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> Set { get; }
        void Insert(T entity);
        void Delete(T entity);
        void Update(T entity);
        T GetById(params object[] keys);
    }
}