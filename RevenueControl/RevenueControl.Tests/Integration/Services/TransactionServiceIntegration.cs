using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevenueControl.DataAccess;
using System.Linq;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Services;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects;
using System.Collections.Generic;
using RevenueControl.InquiryFileReaders.Csv;

namespace RevenueControl.Tests.Integration.Services
{
    [TestClass]
    public class TransactionServiceIntegration
    {

        const string c_clientName = "TestTransactionService";
        const string resourceFile = "Tranzactii_pe_perioada.csv";
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


        private bool CreateClient(bool toUpper = false)
        {
            ActionResponse<Client> result;
            IRepository<Client> clientRepo = new Repository<Client>();
            using (IClientManager clientManager = new ClientManager(clientRepo))
            {
                Client newClient = new Client
                {
                    Name = toUpper ? c_clientName.ToUpper() : c_clientName
                };
                result = clientManager.AddNewClient(newClient);
            }
            return result.Status == ActionResponseCode.Success;
        }

        private bool DeleteClient()
        {
            Client client = new Client
            {
                Name = c_clientName
            };
            bool returnValue;
            IRepository<Client> clientRepo = new Repository<Client>();
            using (IClientManager clientManager = new ClientManager(clientRepo))
            {
                clientManager.DeleteClient(client);
                returnValue = clientManager.SearchForClient(client.Name).Status == ActionResponseCode.NotFound;
            }
            return returnValue;
        }

        private DataSource CreateDataSource()
        {
            IRepository<DataSource> dsRepo = new Repository<DataSource>();
            using (IDataSourceManager dsManager = new DataSourceManager(dsRepo))
            {
                DataSource dataSource = new DataSource
                {
                    BankAccount = "RO75SMB0000999901728781",
                    ClientName = c_clientName,
                    Culture = "ro-RO"
                };
                ActionResponse<DataSource> result = dsManager.CreateDataSource(dataSource);
                return result.Result;
            }
        }

        IList<Transaction> LoadTransactions(DataSource dataSource)
        {
            ITransactionFileReader fileReader = new GenericCsvReader();
            IRepository <DataSource> dsRepo = new Repository<DataSource>();
            IRepository<Transaction> trRepo = new Repository<Transaction>();
            using (ITransactionManager trManager = new TransactionsManager(dsRepo, trRepo, fileReader))
            {
                trManager.AddTransactionsToDataSource(dataSource, GlobalSettings.GetResourceFilePath(resourceFile));
                return trManager.GetDataSourceTransactions(dataSource).ResultList;
            }
        }

        [TestMethod]
        public void TransactionServiceIntegrationTest()
        {

            
            try
            {
                Assert.IsTrue(CreateClient());
                Assert.IsFalse(CreateClient());
                Assert.IsFalse(CreateClient(true));
                DataSource dataSource = CreateDataSource();
                Assert.IsTrue(dataSource != null);
                Assert.IsTrue(dataSource.Id > 0);

            }
            finally
            {
                Assert.IsTrue(DeleteClient());
            }
        }

        //[TestMethod]
        public void DataSourceReadTest()
        {
            IRepository<Client> clientRepo = new Repository<Client>();
            IClientManager clientManager = new ClientManager(clientRepo);
            ActionResponse<Client> response = clientManager.SearchForClient(c_clientName);
            Assert.IsTrue(response.Result.Name == c_clientName);

            IRepository<DataSource> dsRepo = new Repository<DataSource>();
            IDataSourceManager dsManager = new DataSourceManager(dsRepo);

            ActionResponse<DataSource> dataSources = dsManager.GetClientDataSources(response.Result);
            Assert.IsTrue(dataSources.ResultList.Count >= 2);


            DataSource dataSource1 = dataSources.ResultList.Single(ds => ds.BankAccount == "RO75INGB0000999901728780");
            Assert.IsTrue(dataSource1.Name == "Cont Curent");
            Assert.IsTrue(dataSource1.Culture == "ro-RO");
            Assert.IsTrue(dataSource1.ClientName == c_clientName);
            Assert.IsTrue(dataSource1.Id > 0);

            DataSource dataSource2 = dataSources.ResultList.Single(ds => ds.BankAccount == "RO45INGB0000999905243630");
            Assert.IsTrue(dataSource2.Name == "Credit Card");
            Assert.IsTrue(dataSource2.Culture == "ro-RO");
            Assert.IsTrue(dataSource2.ClientName == c_clientName);
            Assert.IsTrue(dataSource2.Id > 0);


            IRepository<Transaction> transactionRepository = new Repository<Transaction>();
            ITransactionManager transactionManager = new TransactionsManager(null, transactionRepository, null);
            ActionResponse<Transaction> transactions = transactionManager.GetDataSourceTransactions(dataSource1);
            Assert.IsTrue(transactions.ResultList.Count == 2);
            Assert.IsTrue(transactions.ResultList[0] == ds1Transaction1);
            Assert.IsTrue(transactions.ResultList[1] == ds1Transaction2);

            transactions = transactionManager.GetDataSourceTransactions(dataSource2);
            Assert.IsTrue(transactions.ResultList.Count == 1);
            Assert.IsTrue(transactions.ResultList[0] == ds2Transaction1);

            ActionResponse<DataSource> searched = dsManager.GetClientDataSources(response.Result, "75ingb");
            Assert.IsTrue(searched.ResultList.Count == 1 && searched.ResultList[0].BankAccount == "RO75INGB0000999901728780");
        }

       
    }
}
