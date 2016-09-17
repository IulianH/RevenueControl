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
        const string ds1BankAccount = "RO75SMB0000999901728781";
        const string ds1Culture = "ro-RO";
        const string ds1Name = "Cont Curent";

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
            using (IClientManager clientManager = new ClientManager(new UnitOfWork()))
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
            using (IClientManager clientManager = new ClientManager(new UnitOfWork()))
            {
                clientManager.DeleteClient(client);
                returnValue = clientManager.SearchForClient(client.Name).Status == ActionResponseCode.NotFound;
            }
            return returnValue;
        }

        private DataSource CreateDataSource()
        {
            using (IDataSourceManager dsManager = new DataSourceManager(new UnitOfWork()))
            {
                DataSource dataSource = new DataSource
                {
                    BankAccount = ds1BankAccount,
                    ClientName = c_clientName,
                    Culture = ds1Culture,
                    Name = ds1Name
                };
                ActionResponse<DataSource> result = dsManager.CreateDataSource(dataSource);
                return result.Result;
            }
        }

        int LoadTransactions(DataSource dataSource)
        {
            ITransactionFileReader fileReader = new GenericCsvReader();
           
            using (ITransactionManager trManager = new TransactionsManager(new UnitOfWork(), fileReader))
            {
                return trManager.AddTransactionsToDataSource(dataSource, GlobalSettings.GetResourceFilePath(resourceFile)).Result;
            }
        }

        IList<Transaction> GetAllTransactions(DataSource dataSource)
        {
            using (ITransactionManager trManager = new TransactionsManager(new UnitOfWork(), null))
            {
                return trManager.GetDataSourceTransactions(dataSource).ResultList;
            }
        }

        IList<DataSource> SearchDataSource(string searchTerm)
        {
            using (IDataSourceManager dsManager = new DataSourceManager(new UnitOfWork()))
            {
                return dsManager.GetClientDataSources(new Client { Name = c_clientName }, searchTerm).ResultList;
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
                IList<DataSource> dsList = SearchDataSource(ds1Name.Substring(1, 7).ToLower());
                Assert.IsTrue(dsList.Count == 1);
                dataSource = dsList[0];
                Assert.IsTrue(dataSource.Name == ds1Name);
                Assert.IsTrue(dataSource.BankAccount == ds1BankAccount);
                Assert.IsTrue(dataSource.Culture == ds1Culture);
                int transactionCount = LoadTransactions(dataSource);
                Assert.IsTrue(transactionCount == 3);
                transactionCount = LoadTransactions(dataSource);
                Assert.IsTrue(transactionCount == 0);
                IList<Transaction> transactions = GetAllTransactions(dataSource);
                Assert.IsTrue(transactions.Count == 3);
            }
            finally
            {
               DeleteClient();
            }
        }
    }
}
