using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects.Entities;
using System.Globalization;

namespace RevenueControl.DomainObjects.Interfaces
{
    public interface ITransactionFileReader
    {
        bool Success { get; }

        IList<Transaction> Read(string fileName, CultureInfo culture);

        IList<Transaction> Read(string fileName, Period period, CultureInfo culture);
    }
}
