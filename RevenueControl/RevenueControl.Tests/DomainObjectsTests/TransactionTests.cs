using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.Tests.DomainObjectsTests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void TransactionEquality()
        {
            // Arrange
            Transaction transaction1 = new Transaction
            {
                Amount = 3,
                OtherDetails = "Other details",
                TransactionDate = new DateTime(2013, 3, 3),
                TransactionDetails = "Details",
                TransactionType = TransactionType.Credit
            };

            Transaction transaction2 = new Transaction
            {
                Amount = 3,
                OtherDetails = "Other details",
                TransactionDate = new DateTime(2013, 3, 3),
                TransactionDetails = "Details",
                TransactionType = TransactionType.Credit
            };

            Transaction transaction3 = new Transaction
            {
                Amount = 3,
                OtherDetails = "other details",
                TransactionDate = new DateTime(2013, 3, 3),
                TransactionDetails = "Details",
                TransactionType = TransactionType.Credit
            };

            // Act
            bool oneVsOne1 = transaction1.Equals(transaction1);
            bool oneVsOne2 = transaction1 == transaction1;

            bool oneVsTwo1 = transaction1.Equals(transaction2);
            bool oneVsTwo2 = transaction1 == transaction2;
        

            bool oneVsThree1 = transaction1.Equals(transaction3);
            bool oneVsThree2 = transaction1 == transaction3;
            bool oneVsThree3 = transaction1 != transaction3;

            // Assert
            Assert.IsTrue(oneVsOne1);
            Assert.IsTrue(oneVsOne2);
            Assert.IsTrue(oneVsTwo1);
            Assert.IsTrue(oneVsTwo2);
            Assert.IsFalse(oneVsThree1);
            Assert.IsFalse(oneVsThree2);
            Assert.IsTrue(oneVsThree3);

        }


    }
}
