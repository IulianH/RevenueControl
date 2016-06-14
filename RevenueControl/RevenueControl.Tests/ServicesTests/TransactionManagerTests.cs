using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.InquiryFileReaders.Csv;
using System.Collections.Generic;
using RevenueControl.Services;

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
            var moq = new Mock<ITransactionRepository>();
            
            DataSource dataSource = new DataSource();
            ITransactionFileReader reader = new GenericCsvReader();
            if(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("en-US")).Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }

            moq.Setup(inst => inst.GetDataSourceTransactions(dataSource, It.IsAny<Period>())).Returns(reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("en-US")));
            TransactionsManager transactionManager = new TransactionsManager(moq.Object, new GenericCsvReader(), new Client { ClientId = 1 }, new System.Globalization.CultureInfo("en-US"));

            // Act
            int count = transactionManager.AddTransactionsToDataSource(dataSource, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(count == 0);
        }

        [TestMethod]
        public void TestAddTransactions()
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";
            var moq = new Mock<ITransactionRepository>();

            DataSource dataSource = new DataSource();
            ITransactionFileReader reader = new GenericCsvReader();
            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("en-US"));
            if(transactions.Count == 0)
            {
                Assert.Inconclusive("Zero transactions read from file");
            }
            TransactionsManager transactionManager = new TransactionsManager(moq.Object, new GenericCsvReader(), new Client { ClientId = 1 }, new System.Globalization.CultureInfo("en-US"));

            // Act
            int count = transactionManager.AddTransactionsToDataSource(dataSource, GlobalSettings.GetResourceFilePath(resourceFile));

            // Assert
            Assert.IsTrue(count == transactions.Count);
        }
    }
}
