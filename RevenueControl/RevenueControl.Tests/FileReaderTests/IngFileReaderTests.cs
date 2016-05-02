using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevenueControl.InquiryFileReaders.Ing;
using RevenueControl.DomainObjects.Entities;
using System.Linq;

namespace RevenueControl.Tests.FileReaderTests
{
    /// <summary>
    /// Summary description for IngFileReaderTests
    /// </summary>
    [TestClass]
    public class IngFileReaderTests
    {
        public IngFileReaderTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
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

        [TestMethod]
        public void IngFileReaderTestMethod1()
        {
            //
            // TODO: Add test logic here
            //

            const string resourceFile = "Inquiry_statements.csv";
            IngFileReader reader = new IngFileReader();

            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("en-US"));
            Transaction firstTransaction = transactions[0];
            Assert.IsTrue(firstTransaction.TransactionDate == new DateTime(2016, 4, 1) && firstTransaction.TransactionType == TransactionType.Debit && firstTransaction.TransactionDetails == "Foreign exchange Home'Bank" &&
                firstTransaction.Amount == 693.17M);
            Transaction lastTransaction = transactions[transactions.Count - 1];
            Assert.IsTrue(lastTransaction.TransactionDate == new DateTime(2016, 2, 4) && lastTransaction.TransactionType == TransactionType.Credit && lastTransaction.TransactionDetails == "Incoming funds" && lastTransaction.Amount == 2500m);
        }

        [TestMethod]
        public void IngFileReaderTestMethod2()
        {
            const string resourceFile = "Tranzactii_pe_perioada.csv";
            IngFileReader reader = new IngFileReader();

            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("ro-RO"));
            Assert.IsTrue(transactions.Count == 3);
            Transaction lastTransaction = transactions[transactions.Count - 1];
            Assert.IsTrue(lastTransaction.TransactionDate == new DateTime(2016, 4, 29) && lastTransaction.TransactionType == TransactionType.Debit && lastTransaction.TransactionDetails == "Cumparare POS" && lastTransaction.Amount == 13.9m);
        }

        [TestMethod]
        public void IngFileReaderTestMethod3()
        {
            const string resourceFile = "Tranzactii_pe_perioada.csv";
            IngFileReader reader = new IngFileReader();

            IList<Transaction> transactions = reader.Read(GlobalSettings.GetResourceFilePath(resourceFile), new System.Globalization.CultureInfo("ro-RO"));
            decimal totalDebits = transactions.Where(t => t.TransactionType == TransactionType.Debit).Sum(t => t.Amount);
            Assert.IsTrue(totalDebits == 257.57m);
        }
    }
}
