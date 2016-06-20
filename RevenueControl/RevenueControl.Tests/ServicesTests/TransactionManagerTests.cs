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

namespace RevenueControl.Tests.ServicesTests
{
    [TestClass]
    public class TransactionManagerTests
    {
        [TestMethod]
        public void TestAddTheSameTransactionsAgain()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            const int clientId = 1;
            const int dataSourceId = 5;
            ITransactionFileReader reader = new GenericCsvReader();
            if(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("en-US")).Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            moqTrRepo.Setup(inst => inst.GetDataSourceTransactions(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<Period>()))
                .Returns(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("en-US")));

            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = clientId, Id = dataSourceId });

            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader(), new Client { Id = clientId }, new System.Globalization.CultureInfo("en-US"));

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource {Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == 0);
        }

        [TestMethod]
        public void TestAddSomeTransactions()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            const int clientId = 1;
            const int dataSourceId = 5;
    
            ITransactionFileReader reader = new GenericCsvReader();
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("en-US"));

            if(transactions.Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = clientId, Id = dataSourceId });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader(), new Client { Id = clientId }, new System.Globalization.CultureInfo("en-US"));

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource {Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == transactions.Count);
        }

        [TestMethod]
        public void TestAddAnEmptyTransactionFile()
        {
            // Arrange
            const string resourceFile = "Tranzactii_pe_perioada_empty.csv";
                                        
            const int clientId = 1;
            const int dataSourceId = 5;
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("ro-RO");

            ITransactionFileReader reader = new GenericCsvReader();
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), culture);
            if (transactions.Count != 0)
            {
                Assert.Inconclusive("The transaction file is not empty");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = clientId, Id = dataSourceId });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader(), new Client { Id = clientId }, new System.Globalization.CultureInfo("en-US"));

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.NoActionPerformed);
            Assert.IsTrue(response.ActionResponseMessage == Localization.GetZeroTransactionsInFile(culture));
        }

        [TestMethod]
        public void TestAddTransactionsToInvalidDataSource()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            const int clientId = 1;
            const int dataSourceId = 5;

            ITransactionFileReader reader = new GenericCsvReader();
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("en-US"));

            if (transactions.Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetById(dataSourceId)).Returns(new DataSource { ClientId = 6, Id = dataSourceId });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader(), new Client { Id = clientId }, new System.Globalization.CultureInfo("en-US"));

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.InvalidInput);
        }
    }
}
