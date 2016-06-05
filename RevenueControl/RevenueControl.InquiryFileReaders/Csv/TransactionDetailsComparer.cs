using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.InquiryFileReaders.Csv
{
    class TransactionDetailsComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return String.Compare(x, y, CultureInfo.InvariantCulture, CompareOptions.Ordinal);
        }
    }
}
