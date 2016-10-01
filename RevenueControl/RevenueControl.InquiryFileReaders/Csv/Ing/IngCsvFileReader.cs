using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using CsvHelper;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Resource;

namespace RevenueControl.InquiryFileReaders.Csv.Ing
{
    public class IngCsvFileReader : ITransactionFileReader
    {
        public IList<Transaction> Read(string fileName, CultureInfo culture)
        {
            return Read(fileName, GlobalConstants.MaxPeriod, culture);
        }

        public IList<Transaction> Read(string fileName, Period period, CultureInfo culture)
        {
            var returnValue = new List<Transaction>();
            var lines = new List<CsvFileLine>();
            using (var sr = new StreamReader(fileName))
            {
                using (var reader = new CsvReader(sr))
                {
                    HybridDictionary maps = null;

                    while (reader.Read())
                    {
                        if (maps == null)
                            maps = CreateColumnMaps(reader, culture);
                        var fileLine = GenericCsvReader.CreateFileLine(reader, maps);
                        if ((fileLine.Date != string.Empty) && (lines.Count > 0))
                        {
                            var transaction = GenericCsvReader.GetTransactionFromLines(lines, culture);
                            if (transaction == null)
                            {
                                returnValue = null;
                                break;
                            }
                            returnValue.Add(transaction);
                            lines.Clear();
                        }
                        lines.Add(fileLine);
                    }
                }
                if ((returnValue != null) && (lines.Count > 0))
                {
                    var transaction = GenericCsvReader.GetTransactionFromLines(lines, culture);
                    if (transaction == null)
                        returnValue = null;
                    returnValue.Add(transaction);
                }
            }
            return returnValue;
        }

        private static HybridDictionary CreateColumnMaps(ICsvReader reader, CultureInfo culture)
        {
            var returnValue = new HybridDictionary();
            var date = Resources.ResourceManager.GetString("Date", culture);
            var transactionDetails = Resources.ResourceManager.GetString("TransactionDetails", culture);
            var debit = Resources.ResourceManager.GetString("Debit", culture);

            var credit = Resources.ResourceManager.GetString("Credit", culture);
            var headers = reader.FieldHeaders;
            for (var i = 0; i < headers.Length; i++)
                if (headers[i] == date)
                    returnValue["Date"] = i - 1;
                else if (headers[i] == transactionDetails)
                    returnValue["TransactionDetails"] = i - 1;
                else if (headers[i] == debit)
                    returnValue["Debit"] = i - 1;
                else if (headers[i] == credit)
                    returnValue["Credit"] = i - 1;
            return returnValue;
        }
    }
}