using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevenueControl.DataAccess;
using System.Linq;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects;

namespace RevenueControl.Tests.Integration.Services
{
    [TestClass]
    public class SingleDbIntegration
    {
        const string clientName = "DefaultClient";
        readonly Transaction ds1Transaction1 = new Transaction
        {
            Amount = 189.67M,
            DataSourceId = 1,
            OtherDetails = "Terminal: SELGROS - 155 ID04  RO  IASI",
            TransactionDate = new DateTime(2016, 4, 27),
            TransactionDetails = "Cumparare POS",
            TransactionType = TransactionType.Debit
        };

        readonly Transaction ds1Transaction2 = new Transaction
        {
            Amount = 54M,
            DataSourceId = 1,
            OtherDetails = "Terminal: RESTAURANT DIONISOS  RO  IASI",
            TransactionDate = new DateTime(2016, 4, 29),
            TransactionDetails = "Cumparare POS",
            TransactionType = TransactionType.Debit
        };

        readonly Transaction ds2Transaction1 = new Transaction
        {
            Amount = 12M,
            DataSourceId = 2,
            OtherDetails = "Salariu",
            TransactionDate = new DateTime(2016, 4, 28),
            TransactionDetails = "Virament",
            TransactionType = TransactionType.Credit
        };


        [TestMethod]
        public void DataSourceReadTest()
        {
            IClientRepository clientRepo = new ClientRepository();
            IClientManager clientManager = new ClientManager(clientRepo);
            ActionResponse<Client> response = clientManager.SearchForClient(clientName);
            Assert.IsTrue(response.Result.Name == clientName);

            IDataSourceRepository dsRepo = new DataSourceRepository();
            IDataSourceManager dsManager = new DataSourceManager(dsRepo);

            ActionResponse<DataSource> dataSources = dsManager.GetClientDataSources(response.Result);
            Assert.IsTrue(dataSources.ResultList.Count >= 2);


            DataSource dataSource1 = dataSources.ResultList.Single(ds => ds.BankAccount == "RO75INGB0000999901728780");
            Assert.IsTrue(dataSource1.Name == "Cont Curent");
            Assert.IsTrue(dataSource1.Culture == "ro-RO");
            Assert.IsTrue(dataSource1.ClientName == clientName);
            Assert.IsTrue(dataSource1.Id > 0);

            DataSource dataSource2 = dataSources.ResultList.Single(ds => ds.BankAccount == "RO45INGB0000999905243630");
            Assert.IsTrue(dataSource2.Name == "Credit Card");
            Assert.IsTrue(dataSource2.Culture == "ro-RO");
            Assert.IsTrue(dataSource2.ClientName == clientName);
            Assert.IsTrue(dataSource2.Id > 0);



            ITransactionRepository transactionRepository = new TransactionRepository();
            ITransactionManager transactionManager = new TransactionsManager(null, transactionRepository, null);
            ActionResponse<Transaction> transactions = transactionManager.GetDataSourceTransactions(dataSource1);
            Assert.IsTrue(transactions.ResultList.Count == 2);
            Assert.IsTrue(transactions.ResultList[0] == ds1Transaction1);
            Assert.IsTrue(transactions.ResultList[1] == ds1Transaction2);

            transactions = transactionManager.GetDataSourceTransactions(dataSource2);
            Assert.IsTrue(transactions.ResultList.Count == 1);
            Assert.IsTrue(transactions.ResultList[0] == ds2Transaction1);
        }

       
    }
}
