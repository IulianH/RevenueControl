using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.InquiryFileReaders.Csv;
using System.Collections.Generic;
using RevenueControl.Services;
using RevenueControl.DomainObjects;
using RevenueControl.Resources;
using System.Globalization;
using System.Linq;

namespace RevenueControl.Tests.ServicesTests
{
    [TestClass]
    public class TransactionManagerTests
    {
        [TestMethod]
        public void AddTheSameTransactionsAgain()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            const int clientId = 1;
            const int dataSourceId = 5;
            CultureInfo culture = new CultureInfo("en-US");
            ITransactionFileReader reader = new GenericCsvReader();
            if(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), culture).Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            moqTrRepo.Setup(inst => inst.GetDataSourceTransactions(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<Period>()))
                .Returns(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), culture));

            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = clientId, Id = dataSourceId });

            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader(), new Client { Id = clientId }, culture);

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource {Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == 0);
        }

        [TestMethod]
        public void AddNewTransactions()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            const int clientId = 1;
            const int dataSourceId = 5;
            CultureInfo culture = new CultureInfo("en-US");

            ITransactionFileReader reader = new GenericCsvReader();
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), culture);

            if(transactions.Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = clientId, Id = dataSourceId });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader(), new Client { Id = clientId }, culture);

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource {Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == transactions.Count);
        }

        [TestMethod]
        public void AddAnEmptyTransactionFile()
        {
            // Arrange
            const string resourceFile = "Tranzactii_pe_perioada_empty.csv";
                                        
            const int clientId = 1;
            const int dataSourceId = 5;
            System.Globalization.CultureInfo culture = new CultureInfo("ro-RO");

            ITransactionFileReader reader = new GenericCsvReader();
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), culture);
            if (transactions.Count != 0)
            {
                Assert.Inconclusive("The transaction file is not empty");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = clientId, Id = dataSourceId });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader(), new Client { Id = clientId }, culture);

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.NoActionPerformed);
            Assert.IsTrue(response.ActionResponseMessage == Localization.GetZeroTransactionsInFile(culture));
        }

        [TestMethod]
        public void AddTransactionsToInvalidDataSource()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            const int clientId = 1;
            const int dataSourceId = 5;
            CultureInfo culture = new CultureInfo("en-US");

            ITransactionFileReader reader = new GenericCsvReader();
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), culture);

            if (transactions.Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = 6, Id = dataSourceId });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader(), new Client { Id = clientId }, culture);

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.InvalidInput);
        }

        [TestMethod]
        public void AddSomeTransactions()
        {
            // Arrange
            Transaction[] toAdd = 
            {
                new Transaction
                {
                    Amount = 2,
                    OtherDetails = "other details",
                    TransactionDate = new DateTime(2016, 3, 3),
                    TransactionDetails = "transaction details",
                    TransactionType = TransactionType.Credit
                },
                new Transaction
                {
                    Amount = 2,
                    OtherDetails = "details",
                    TransactionDate = new DateTime(2016, 3, 3),
                    TransactionDetails = "transaction details",
                    TransactionType = TransactionType.Credit
                }
            };

            Transaction[] existing =
          {
                new Transaction
                {
                    Amount = 2,
                    OtherDetails = "other details",
                    TransactionDate = new DateTime(2016, 3, 3),
                    TransactionDetails = "transaction details",
                    TransactionType = TransactionType.Credit
                }
            };

         
            const string resourceFile = "Inquiry_statements.csv";
            const int clientId = 1;
            const int dataSourceId = 5;
            CultureInfo culture = new CultureInfo("en-US");

            var moqFileReader = new Mock<ITransactionFileReader>();
            moqFileReader.Setup(inst => inst.Read(GlobalSettings.GetResourceFilePath(resourceFile), culture)).Returns(toAdd);

            var moqTrRepo = new Mock<ITransactionRepository>();
            moqTrRepo.Setup(inst => inst.GetDataSourceTransactions(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<Period>()))
                .Returns(existing);

            IEnumerable<Transaction> passedToRepository = null;
            moqTrRepo.Setup(inst => inst.AddTransactionsToDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<IEnumerable<Transaction>>()))
                .Callback<DataSource, IEnumerable<Transaction>>((ds, list) => passedToRepository = list);

            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = clientId, Id = dataSourceId });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, moqFileReader.Object, new Client { Id = clientId }, culture);

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == 1);
            Assert.IsTrue(passedToRepository.Single() == toAdd[1]);
        }
    }
}
