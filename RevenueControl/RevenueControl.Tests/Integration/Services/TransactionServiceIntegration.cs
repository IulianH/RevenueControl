using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevenueControl.DataAccess;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.InquiryFileReaders.Csv;
using RevenueControl.Services;

namespace RevenueControl.Tests.Integration.Services
{
    [TestClass]
    public class TransactionServiceIntegration
    {
        private const string c_clientName = "TestTransactionService";
        private const string resourceFile = "Tranzactii_pe_perioada.csv";
        private const string ds1BankAccount = "RO75SMB0000999901728781";
        private const string ds1Culture = "ro-RO";
        private const string ds1Name = "Cont Curent";
        private readonly string[] Tags = {"Tag1", "Tag2"};

        private static DataSource Ds1 => new DataSource
        {
            BankAccount = ds1BankAccount,
            ClientName = c_clientName,
            Culture = ds1Culture,
            Name = ds1Name
        };

        private static Client Client => new Client {Name = c_clientName};


        private bool CreateClient(bool toUpper = false)
        {
            ActionResponse result;
            using (IClientManager clientManager = new ClientManager(new UnitOfWork()))
            {
                var newClient = new Client
                {
                    Name = toUpper ? c_clientName.ToUpper() : c_clientName
                };
                result = clientManager.AddNew(newClient);
            }
            return result.Status == ActionResponseCode.Success;
        }


        private static bool DeleteClient()
        {
            var client = new Client
            {
                Name = c_clientName
            };
            bool returnValue;
            using (IClientManager clientManager = new ClientManager(new UnitOfWork()))
            {
                clientManager.Delete(client);
                returnValue = clientManager.GetById(client.Name) == null;
            }
            return returnValue;
        }

        private DataSource CreateDataSource()
        {
            using (IDataSourceManager dsManager = new DataSourceManager(new UnitOfWork()))
            {
                var ds1 = Ds1;
                var result = dsManager.Insert(ds1);
                return ds1;
            }
        }

        private void LoadTransactions(DataSource dataSource)
        {
            ITransactionFileReader fileReader = new GenericCsvReader();

            using (ITransactionManager trManager = new TransactionsManager(new UnitOfWork(), fileReader))
            {
                trManager.Insert(dataSource, GlobalSettings.GetResourceFilePath(resourceFile));
            }
        }

        private IList<Transaction> GetAllTransactions(DataSource dataSource)
        {
            using (ITransactionManager trManager = new TransactionsManager(new UnitOfWork(), null))
            {
                return trManager.Get(dataSource);
            }
        }

        private bool TagTransaction(Transaction transaction)
        {
            using (ITransactionManager trManager = new TransactionsManager(new UnitOfWork(), null))
            {
                return trManager.TagTransaction(transaction.Id, Tags).Status == ActionResponseCode.Success;
            }
        }

        private IList<DataSource> SearchDataSource(string searchTerm)
        {
            using (IDataSourceManager dsManager = new DataSourceManager(new UnitOfWork()))
            {
                return dsManager.Get(c_clientName, searchTerm);
            }
        }

        private bool HasDataSources()
        {
            using (IClientManager clientManager = new ClientManager(new UnitOfWork()))
            {
                return clientManager.HasDataSources(Client);
            }
        }

        private bool HasTransactions(DataSource dataSource)
        {
            using (IDataSourceManager dsManager = new DataSourceManager(new UnitOfWork()))
            {
                return dsManager.HasTransactions(dataSource);
            }
        }

        private Client GetClientById()
        {
            using (IClientManager clientManager = new ClientManager(new UnitOfWork()))
            {
                return clientManager.GetById(c_clientName);
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
                var client = GetClientById();
                Assert.IsTrue(client != null);
                var dataSource = CreateDataSource();
                Assert.IsTrue(dataSource != null);
                Assert.IsTrue(HasDataSources());
                Assert.IsTrue(dataSource.Id > 0);
                Assert.IsFalse(HasTransactions(dataSource));
                var dsList = SearchDataSource(ds1Name.Substring(1, 7).ToLower());
                Assert.IsTrue(dsList.Count == 1);
                Assert.IsTrue(dsList[0].GetType() == typeof(DataSource));
                dataSource = dsList[0];
                Assert.IsTrue(dataSource.Name == ds1Name);
                Assert.IsTrue(dataSource.BankAccount == ds1BankAccount);
                Assert.IsTrue(dataSource.Culture == ds1Culture);
                LoadTransactions(dataSource);
                Assert.IsTrue(HasTransactions(dataSource));
                Assert.IsTrue(GetAllTransactions(dataSource).Count == 3);
                LoadTransactions(dataSource);
                var transactions = GetAllTransactions(dataSource);
                Assert.IsTrue(transactions.Count == 3);
                Assert.IsTrue(transactions[0].GetType() == typeof(Transaction));
                Assert.IsTrue(TagTransaction(transactions.Last()));
                transactions = GetAllTransactions(dataSource);
                Assert.IsTrue(transactions.Last().Tags.Count == 2);
                Assert.IsTrue(transactions.First().Tags.Count == 0);
            }
            finally
            {
                DeleteClient();
            }
        }
    }
}