using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Entities
{
    public enum TransactionType
    {
        Debit = 0,
        Credit
    }

    public class Transaction 
    {
        public DataSource DataSource { get; set; }

        public DateTime TransactionDate { get; set; }

        public string TransactionDetails { get; set; }

        public decimal Amount { get; set; }
        
        public TransactionType TransactionType { get; set; }

        public string OtherDetails { get; set; }

        

        private bool Equals(Transaction other)
        {
            return TransactionDate == other.TransactionDate && TransactionDetails == other.TransactionDetails && Amount == other.Amount && TransactionType == other.TransactionType && OtherDetails == other.OtherDetails;
        }

        public static bool operator ==(Transaction a, Transaction b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            return a.Equals(b);            
        }

        public override bool Equals(object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            Transaction p = obj as Transaction;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return base.Equals(obj) && Equals(p);
        }

        public static bool operator !=(Transaction a, Transaction b)
        {
            return !(a == b);
        }

        private int _hash = int.MinValue;
        public override int GetHashCode()
        {
            if(_hash == int.MinValue)
            {
                _hash = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", this.TransactionDate.Year, TransactionDate.Month, TransactionDate.Day, TransactionDetails, Amount).GetHashCode();
            }
            return _hash;
        }
    }
}
