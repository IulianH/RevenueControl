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

        DataSource Ds1
        {
            get
            {
                return new DataSource
                {
                    BankAccount = ds1BankAccount,
                    ClientName = c_clientName,
                    Culture = ds1Culture,
                    Name = ds1Name
                };
            }

        }

        Client Client
        {
            get
            {
                return new Client { Name = c_clientName };
            }
            
        }

        private DataSource CreateDataSource()
        {
            using (IDataSourceManager dsManager = new DataSourceManager(new UnitOfWork()))
            {
                ActionResponse<DataSource> result = dsManager.CreateDataSource(Ds1);
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
                return dsManager.GetClientDataSources(Client, searchTerm).ResultList;
            }
        }

        bool HasDataSources()
        {
            using (IClientManager clientManager = new ClientManager(new UnitOfWork()))
            {
                return clientManager.HasDataSources(Client);
            }
        }

        bool HasTransactions(DataSource dataSource)
        {
            using (IDataSourceManager dsManager = new DataSourceManager(new UnitOfWork()))
            {
                return dsManager.HasTransactions(dataSource);
            }
        }

        [TestMethod]
        public void TransactionServiceIntegrationTest()
        {
            try
            {
                Assert.IsTrue(CreateClient());
                Assert.IsFalse(HasDataSources());
                Assert.IsFalse(CreateClient());
                Assert.IsFalse(CreateClient(true));
                DataSource dataSource = CreateDataSource();
                Assert.IsTrue(dataSource != null);
                Assert.IsTrue(HasDataSources());
                Assert.IsTrue(dataSource.Id > 0);
                Assert.IsFalse(HasTransactions(dataSource));
                IList<DataSource> dsList = SearchDataSource(ds1Name.Substring(1, 7).ToLower());
                Assert.IsTrue(dsList.Count == 1);
                dataSource = dsList[0];
                Assert.IsTrue(dataSource.Name == ds1Name);
                Assert.IsTrue(dataSource.BankAccount == ds1BankAccount);
                Assert.IsTrue(dataSource.Culture == ds1Culture);
                int transactionCount = LoadTransactions(dataSource);
                Assert.IsTrue(HasTransactions(dataSource));
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
