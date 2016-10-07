using System;
using System.Collections.Generic;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface ITransactionManager : IDisposable
    {
        ActionResponse Insert(DataSource dataSource, string transactionReportFile);

        ActionResponse Insert(DataSource dataSource, string transactionReportFile, Period period);

        IList<Transaction> Get(DataSource dataSource, string searchTerm = null);

        IList<Transaction> Get(DataSource dataSource, Period period, string searchTerm = null);

        IList<Transaction> GetAll(DataSource dataSource, string searchTerm = null);

        IList<Transaction> GetAll(DataSource dataSource, Period period, string searchTerm = null);
    }
}