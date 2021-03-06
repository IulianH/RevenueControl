﻿using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.InquiryFileReaders.Csv;
using RevenueControl.InquiryFileReaders.Csv.Ing;

namespace RevenueControl.Tests.FileReaderTests
{
    /// <summary>
    ///     Summary description for IngFileReaderTests
    /// </summary>
    [TestClass]
    public class CsvReaderTests
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void IngEnfile()
        {
            TestMethodEnFile(new IngCsvFileReader());
        }

        private void TestMethodEnFile(ITransactionFileReader reader)
        {
            // Arrange
            const string resourceFile = "Inquiry_statements.csv";

            // Act
            var transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo("en-US"));

            // Assert
            var firstTransaction = transactions[0];
            Assert.IsTrue((firstTransaction.TransactionDate == new DateTime(2016, 4, 1)) &&
                          (firstTransaction.TransactionType == TransactionType.Debit) &&
                          (firstTransaction.TransactionDetails == "Foreign exchange Home'Bank") &&
                          (firstTransaction.Amount == 693.17M));
            var lastTransaction = transactions[transactions.Count - 1];
            Assert.IsTrue((lastTransaction.TransactionDate == new DateTime(2016, 2, 4)) &&
                          (lastTransaction.TransactionType == TransactionType.Credit) &&
                          (lastTransaction.TransactionDetails == "Incoming funds") && (lastTransaction.Amount == 2500m));
        }


        [TestMethod]
        public void IngRoFile()
        {
            TestMethodRoFile(new IngCsvFileReader());
        }

        private void TestMethodRoFile(ITransactionFileReader reader)
        {
            // Arrange
            const string resourceFile = "Tranzactii_pe_perioada.csv";

            // Act
            var transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo("ro-RO"));

            // Assert
            Assert.IsTrue(transactions.Count == 3);
            var lastTransaction = transactions[transactions.Count - 1];
            Assert.IsTrue((lastTransaction.TransactionDate == new DateTime(2016, 4, 29)) &&
                          (lastTransaction.TransactionType == TransactionType.Debit) &&
                          (lastTransaction.TransactionDetails == "Cumparare POS") && (lastTransaction.Amount == 13.9m));
            var totalDebits = transactions.Where(t => t.TransactionType == TransactionType.Debit).Sum(t => t.Amount);
            Assert.IsTrue(totalDebits == 257.57m);
        }

        [TestMethod]
        public void GenericReaderEnFile()
        {
            TestMethodEnFile(new GenericCsvReader());
        }

        [TestMethod]
        public void GenericReaderRoFile()
        {
            TestMethodRoFile(new GenericCsvReader());
        }

        [TestMethod]
        public void NoCommaAtStartFile()
        {
            // Arrange
            const string resourceFile = "RO75INGB0000999901728780.csv";
            var reader = new GenericCsvReader();

            //Act
            var transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new CultureInfo("en-US"));

            //Assert
            Assert.IsTrue(transactions.Count == 2);
            var transaction = transactions[1];
            Assert.IsTrue(transaction.TransactionDate == new DateTime(2016, 9, 21));
            Assert.IsTrue(transaction.TransactionType == TransactionType.Debit);
            Assert.IsTrue(transaction.Amount == 70.50m);
            Assert.IsTrue(transaction.TransactionDetails == "POS purchase");
            Assert.IsTrue(transaction.OtherDetails.Contains("043740"));
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion
    }
}