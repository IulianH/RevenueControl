using System;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Client> ClientRepository { get; }

        IRepository<DataSource> DataSourceRepository { get; }

        IRepository<Transaction> TransactionRepository { get; }

        void Save();
    }
}