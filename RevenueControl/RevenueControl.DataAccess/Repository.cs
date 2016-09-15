using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects;
using System.Linq.Expressions;

namespace RevenueControl.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected RevenueControlDb db = new RevenueControlDb();
        public void Delete(T entity)
        {
            db.Set<T>().Remove(entity);
            db.SaveChanges();
        }

        public void Insert(T entity)
        {
            db.Set<T>().Add(entity);
            db.SaveChanges();
        }

        public void Update(T entity)
        {
            db.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public IEnumerable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return db.Set<T>().Where(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return db.Set<T>();
        }

        public T GetById(params object[] keys)
        {
            return db.Set<T>().Find(keys);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    db.Dispose();
                    db = null; 
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BaseRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

       
        #endregion


    }
}
