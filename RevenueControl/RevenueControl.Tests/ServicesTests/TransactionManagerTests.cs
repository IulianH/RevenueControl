using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.InquiryFileReaders.Csv;
using RevenueControl.Resource;
using RevenueControl.Services;

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
            const string clientId = "1";
            const int dataSourceId = 5;
            const string cultureStr = "en-US";
            ITransactionFileReader reader = new GenericCsvReader();
            if (reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr)).Count == 0)
                Assert.Inconclusive("Zero transactions read from file");
            var moqTrRepo = new Mock<IRepository<Transaction>>();
            var transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr));
            foreach (var tr in transactions)
                tr.DataSourceId = dataSourceId;
            moqTrRepo.SetupGet(inst => inst.Set).Returns(transactions.AsQueryable());

            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource {ClientName = clientId, Id = dataSourceId, Culture = cultureStr});
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);


            var transactionManager = new TransactionsManager(moqUnitOfWork.Object, new GenericCsvReader());

            // Act
            var response = transactionManager.Insert(new DataSource {Id = dataSourceId, ClientName = clientId},
                GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            moqTrRepo.Verify(inst => inst.Set, Times.AtLeastOnce);
            moqTrRepo.Verify(inst => inst.Insert(It.IsAny<Transaction>()), Times.Never);
            moqDsRepo.Verify(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)), Times.AtLeastOnce);
            Assert.IsTrue(response.Status == ActionResponseCode.Success);
        }

        [TestMethod]
        public void AddNewTransactions()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            const string clientId = "1";
            const int dataSourceId = 5;
            const string cultureStr = "en-US";

            ITransactionFileReader reader = new GenericCsvReader();
            var transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr));

            if (transactions.Count == 0)
                Assert.Inconclusive("Zero transactions read from file");
            var moqTrRepo = new Mock<IRepository<Transaction>>();
            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource {ClientName = clientId, Id = dataSourceId, Culture = cultureStr});
            moqTrRepo.SetupGet(inst => inst.Set).Returns(new Transaction[0].AsQueryable());

            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);
            var transactionManager = new TransactionsManager(moqUnitOfWork.Object, new GenericCsvReader());

            // Act
            var response = transactionManager.Insert(new DataSource {Id = dataSourceId, ClientName = clientId},
                GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            moqTrRepo.Verify(inst => inst.Set, Times.AtLeastOnce);
            moqTrRepo.Verify(inst => inst.Insert(It.IsAny<Transaction>()), Times.Exactly(transactions.Count));
            moqDsRepo.Verify(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)), Times.AtLeastOnce);
            Assert.IsTrue(response.Status == ActionResponseCode.Success);
        }

        [TestMethod]
        public void AddAnEmptyTransactionFile()
        {
            // Arrange
            const string resourceFile = "Tranzactii_pe_perioada_empty.csv";

            const string clientId = "1";
            const int dataSourceId = 5;
            const string cultureStr = "ro-RO";

            ITransactionFileReader reader = new GenericCsvReader();
            var transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr));
            if (transactions.Count != 0)
                Assert.Inconclusive("The transaction file is not empty");
            var moqTrRepo = new Mock<IRepository<Transaction>>();
            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource {ClientName = clientId, Id = dataSourceId, Culture = cultureStr});
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);
            var transactionManager = new TransactionsManager(moqUnitOfWork.Object, new GenericCsvReader());

            // Act
            var response = transactionManager.Insert(new DataSource {Id = dataSourceId, ClientName = clientId},
                GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.NoActionPerformed);
            moqTrRepo.Verify(inst => inst.Insert(It.IsAny<Transaction>()), Times.Never);
            Assert.IsTrue(response.ActionResponseMessage == Resources.ZeroTransactionsInFile);
            moqDsRepo.Verify(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)), Times.AtLeastOnce);
        }

        [TestMethod]
        public void AddTransactionsToInvalidDataSource()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            const string clientId = "1";
            const int dataSourceId = 5;
            const string cultureStr = "en-US";

            ITransactionFileReader reader = new GenericCsvReader();
            var transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr));

            if (transactions.Count == 0)
                Assert.Inconclusive("Zero transactions read from file");
            var moqTrRepo = new Mock<IRepository<Transaction>>();
            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource {ClientName = clientId, Id = dataSourceId, Culture = cultureStr});
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);
            var transactionManager = new TransactionsManager(moqUnitOfWork.Object, new GenericCsvReader());

            // Act
            var response = transactionManager.Insert(new DataSource {Id = 4, ClientName = clientId},
                GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.NotFound);
            moqDsRepo.Verify(inst => inst.GetById(It.Is<int>(id => id == 4)), Times.AtLeastOnce);
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


            const string resourceFile = "Inquiry_statements.csv";
            const string clientId = "1";
            const int dataSourceId = 5;
            const string cultureStr = "ro-RO";

            Transaction[] existing =
            {
                new Transaction
                {
                    Amount = 2,
                    DataSourceId = dataSourceId,
                    OtherDetails = "other details",
                    TransactionDate = new DateTime(2016, 3, 3),
                    TransactionDetails = "transaction details",
                    TransactionType = TransactionType.Credit
                }
            };

            var moqFileReader = new Mock<ITransactionFileReader>();
            moqFileReader.Setup(
                    inst => inst.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr)))
                .Returns(toAdd);

            var moqTrRepo = new Mock<IRepository<Transaction>>();
            moqTrRepo.SetupGet(inst => inst.Set).Returns(existing.AsQueryable());

            var passedToRepository = new List<Transaction>();


            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource {ClientName = clientId, Id = dataSourceId, Culture = cultureStr});
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);
            var transactionManager = new TransactionsManager(moqUnitOfWork.Object, moqFileReader.Object);

            // Act
            var response = transactionManager.Insert(new DataSource {Id = dataSourceId, ClientName = clientId},
                GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            moqTrRepo.Verify(inst => inst.Insert(It.Is<Transaction>(tr => tr == toAdd[1])));
            moqTrRepo.Verify(inst => inst.Set, Times.AtLeastOnce);
            moqTrRepo.Verify(inst => inst.Insert(It.Is<Transaction>(tr => tr == toAdd[1])), Times.Once);
            moqTrRepo.Verify(inst => inst.Insert(It.IsAny<Transaction>()), Times.Once);
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource {ClientName = clientId, Id = dataSourceId, Culture = cultureStr});
            Assert.IsTrue(response.Status == ActionResponseCode.Success);
        }
    }
}