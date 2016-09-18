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
using System.Linq.Expressions;

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
            if(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr)).Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<IRepository<Transaction>>();
            moqTrRepo.Setup(inst => inst.Get(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .Returns(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr)));

            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);


            TransactionsManager transactionManager = new TransactionsManager(moqUnitOfWork.Object, new GenericCsvReader());

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource {Id = dataSourceId, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            moqTrRepo.Verify(inst => inst.Get(It.IsAny<Expression<Func<Transaction, bool>>>()), Times.AtLeastOnce);
            moqDsRepo.Verify(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)), Times.AtLeastOnce);
            Assert.IsTrue(response.Result == 0);
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
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr));

            if(transactions.Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<IRepository<Transaction>>();
            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
               .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);
            TransactionsManager transactionManager = new TransactionsManager(moqUnitOfWork.Object, new GenericCsvReader());

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource {Id = dataSourceId, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == transactions.Count);
            moqTrRepo.Verify(inst => inst.Get(It.IsAny<Expression<Func<Transaction, bool>>>()), Times.AtLeastOnce);
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
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr));
            if (transactions.Count != 0)
            {
                Assert.Inconclusive("The transaction file is not empty");
            }
            var moqTrRepo = new Mock<IRepository<Transaction>>();
            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);
            TransactionsManager transactionManager = new TransactionsManager(moqUnitOfWork.Object, new GenericCsvReader());

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.NoActionPerformed);
            Assert.IsTrue(response.ActionResponseMessage == Localization.GetZeroTransactionsInFile(new CultureInfo(cultureStr)));
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
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr));

            if (transactions.Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<IRepository<Transaction>>();
            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);
            TransactionsManager transactionManager = new TransactionsManager(moqUnitOfWork.Object, new GenericCsvReader());

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = 4, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

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
            const string clientId = "1";
            const int dataSourceId = 5;
            const string cultureStr = "ro-RO";

            var moqFileReader = new Mock<ITransactionFileReader>();
            moqFileReader.Setup(inst => inst.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr))).Returns(toAdd);

            var moqTrRepo = new Mock<IRepository<Transaction>>();
            moqTrRepo.Setup(inst => inst.Get(It.IsAny<Expression<Func<Transaction, bool>>>())).Returns(existing);

            List<Transaction> passedToRepository = new List<Transaction>();
            

            var moqDsRepo = new Mock<IRepository<DataSource>>();
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            var moqUnitOfWork = new Mock<IUnitOfWork>();
            moqUnitOfWork.SetupGet(inst => inst.DataSourceRepository).Returns(moqDsRepo.Object);
            moqUnitOfWork.SetupGet(inst => inst.TransactionRepository).Returns(moqTrRepo.Object);
            TransactionsManager transactionManager = new TransactionsManager(moqUnitOfWork.Object, moqFileReader.Object);

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == 1);
            moqTrRepo.Verify(inst => inst.Get(It.IsAny<Expression<Func<Transaction, bool>>>()), Times.AtLeastOnce);
            moqTrRepo.Verify(inst => inst.Insert(It.Is<Transaction>(tr => tr == toAdd[1])), Times.Once);
            moqTrRepo.Verify(inst => inst.Insert(It.IsAny<Transaction>()), Times.Once);
            moqDsRepo.Setup(inst => inst.GetById(It.Is<int>(id => id == dataSourceId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            Assert.IsTrue(response.Status == ActionResponseCode.Success);
        }
    }
}
