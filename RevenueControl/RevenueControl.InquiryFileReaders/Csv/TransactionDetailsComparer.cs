using System.Collections.Generic;
using System.Globalization;

namespace RevenueControl.InquiryFileReaders.Csv
{
    internal class TransactionDetailsComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, CultureInfo.InvariantCulture, CompareOptions.Ordinal);
        }
    }
}