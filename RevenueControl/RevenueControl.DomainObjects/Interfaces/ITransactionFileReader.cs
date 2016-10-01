using System.Collections.Generic;
using System.Globalization;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface ITransactionFileReader
    {
        IList<Transaction> Read(string fileName, CultureInfo culture);

        IList<Transaction> Read(string fileName, Period period, CultureInfo culture);
    }
}