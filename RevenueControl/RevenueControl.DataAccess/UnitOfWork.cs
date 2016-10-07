using System;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;

namespace RevenueControl.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RevenueControlDb _context = new RevenueControlDb();
        private IRepository<Client> _clientRepository;
        private IRepository<DataSource> _dataSourceRepository;
        private IRepository<Transaction> _transactionRepository;
       


        public IRepository<Client> ClientRepository
        {
            get
            {
                if (_clientRepository == null)
                    _clientRepository = new Repository<Client>(_context);
                return _clientRepository;
            }
        }

        public IRepository<DataSource> DataSourceRepository
        {
            get
            {
                if (_dataSourceRepository == null)
                    _dataSourceRepository = new Repository<DataSource>(_context);
                return _dataSourceRepository;
            }
        }

        public IRepository<Transaction> TransactionRepository
        {
            get
            {
                if (_transactionRepository == null)
                    _transactionRepository = new Repository<Transaction>(_context);
                return _transactionRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    _context.Dispose();

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