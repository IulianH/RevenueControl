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
            const string clientId = "1";
            const int dataSourceId = 5;
            const string cultureStr = "en-US";
            ITransactionFileReader reader = new GenericCsvReader();
            if(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr)).Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            var moqTrRepo = new Mock<ITransactionRepository>();
            moqTrRepo.Setup(inst => inst.GetDataSourceTransactions(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId), It.IsAny<Period>(), null))
                .Returns(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo(cultureStr)));

            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });

            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader());

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource {Id = dataSourceId, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            moqTrRepo.Verify(inst => inst.GetDataSourceTransactions(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<Period>(), null), Times.AtLeastOnce);
            moqDsRepo.Verify(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)), Times.AtLeastOnce);
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
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)))
               .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader());

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource {Id = dataSourceId, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == transactions.Count);
            moqTrRepo.Verify(inst => inst.GetDataSourceTransactions(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<Period>(), null), Times.AtLeastOnce);
            moqDsRepo.Verify(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)), Times.AtLeastOnce);
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
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader());

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.NoActionPerformed);
            Assert.IsTrue(response.ActionResponseMessage == Localization.GetZeroTransactionsInFile(new CultureInfo(cultureStr)));
            moqDsRepo.Verify(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)), Times.AtLeastOnce);
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
            var moqTrRepo = new Mock<ITransactionRepository>();
            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, new GenericCsvReader());

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = 4, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Status == ActionResponseCode.NotFound);
            moqDsRepo.Verify(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == 4 && ds.ClientName == clientId)), Times.AtLeastOnce);
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

            var moqTrRepo = new Mock<ITransactionRepository>();
            moqTrRepo.Setup(inst => inst.GetDataSourceTransactions(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<Period>(), null))
                .Returns(existing);

            IEnumerable<Transaction> passedToRepository = null;
            moqTrRepo.Setup(inst => inst.AddTransactionsToDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<IEnumerable<Transaction>>()))
                .Callback<DataSource, IEnumerable<Transaction>>((ds, list) => passedToRepository = list);

            var moqDsRepo = new Mock<IDataSourceRepository>();
            moqDsRepo.Setup(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            TransactionsManager transactionManager = new TransactionsManager(moqDsRepo.Object, moqTrRepo.Object, moqFileReader.Object);

            // Act
            ActionResponse<int> response = transactionManager.AddTransactionsToDataSource(new DataSource { Id = dataSourceId, ClientName = clientId }, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(response.Result == 1);
            Assert.IsTrue(passedToRepository.Single() == toAdd[1]);
            moqTrRepo.Verify(inst => inst.GetDataSourceTransactions(It.Is<DataSource>(ds => ds.Id == dataSourceId), It.IsAny<Period>(), null), Times.AtLeastOnce);
            moqDsRepo.Setup(inst => inst.GetDataSource(It.Is<DataSource>(ds => ds.Id == dataSourceId && ds.ClientName == clientId)))
                .Returns(new DataSource { ClientName = clientId, Id = dataSourceId, Culture = cultureStr });
            Assert.IsTrue(response.Status == ActionResponseCode.Success);
        }
    }
}
