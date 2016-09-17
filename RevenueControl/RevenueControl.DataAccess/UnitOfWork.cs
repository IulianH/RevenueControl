using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        RevenueControlDb context = new RevenueControlDb();
        IRepository<Client> clientRepository;
        IRepository<DataSource> dataSourceRepository;
        IRepository<Transaction> transactionRepository;


        public IRepository<Client> ClientRepository
        {
            get
            {
                if(clientRepository == null)
                {
                    clientRepository = new Repository<Client>(context);
                }
                return clientRepository;
            }
        }

        public IRepository<DataSource> DataSourceRepository
        {
            get
            {
                if(dataSourceRepository == null)
                {
                    dataSourceRepository = new Repository<DataSource>(context);
                }
                return dataSourceRepository;
            }
        }

        public IRepository<Transaction> TransactionRepository
        {
            get
            {
                if(transactionRepository == null)
                {
                    transactionRepository = new Repository<Transaction>(context);
                }
                return transactionRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
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
                    context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
