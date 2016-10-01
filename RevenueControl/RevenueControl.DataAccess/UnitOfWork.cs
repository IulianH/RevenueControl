using System;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;

namespace RevenueControl.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<Client> clientRepository;
        private readonly RevenueControlDb context = new RevenueControlDb();
        private IRepository<DataSource> dataSourceRepository;
        private IRepository<Transaction> transactionRepository;
        private IRepository<TransactionTag> transactionTagRepository;


        public IRepository<Client> ClientRepository
        {
            get
            {
                if (clientRepository == null)
                    clientRepository = new Repository<Client>(context);
                return clientRepository;
            }
        }

        public IRepository<DataSource> DataSourceRepository
        {
            get
            {
                if (dataSourceRepository == null)
                    dataSourceRepository = new Repository<DataSource>(context);
                return dataSourceRepository;
            }
        }

        public IRepository<Transaction> TransactionRepository
        {
            get
            {
                if (transactionRepository == null)
                    transactionRepository = new Repository<Transaction>(context);
                return transactionRepository;
            }
        }

        public IRepository<TransactionTag> TransactionTagRepository
        {
            get
            {
                if (transactionTagRepository == null)
                    transactionTagRepository = new Repository<TransactionTag>(context);
                return transactionTagRepository;
            }
        }


        public void Save()
        {
            context.SaveChanges();
        }

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    context.Dispose();

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