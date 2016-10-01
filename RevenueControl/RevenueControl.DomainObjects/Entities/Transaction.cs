using System;
using System.Collections.Generic;
using System.Globalization;

namespace RevenueControl.DomainObjects.Entities
{
    public enum TransactionType : byte
    {
        Debit = 1,
        Credit
    }

    public class Transaction
    {
        public int Id { get; set; }

        public DateTime TransactionDate { get; set; }

        public string TransactionDetails { get; set; }

        public decimal Amount { get; set; }

        public TransactionType TransactionType { get; set; }

        public string OtherDetails { get; set; }

        public int DataSourceId { get; set; }

        public ICollection<string> Tags { get; set; }

        public bool Ignore { get; set; }

        private bool Equals(Transaction other)
        {
            return (TransactionDate == other.TransactionDate)
                   && ((TransactionDetails ?? string.Empty) == (other.TransactionDetails ?? string.Empty))
                   && (Amount == other.Amount)
                   && (TransactionType == other.TransactionType)
                   && ((OtherDetails ?? string.Empty) == (other.OtherDetails ?? string.Empty));
        }

        public static bool operator ==(Transaction a, Transaction b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
                return false;
            return a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            // If parameter cannot be cast to Point return false.
            var p = obj as Transaction;
            return ((object) p != null) && Equals(p);

            // Return true if the fields match:
        }

        public static bool operator !=(Transaction a, Transaction b)
        {
            return !(a == b);
        }


        public override int GetHashCode()
        {
            //do not store the hashcode in a variable, it must be dynamic
            return
                string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", TransactionDate.Year,
                    TransactionDate.Month, TransactionDate.Day, TransactionDetails, Amount).GetHashCode();
        }
    }
}