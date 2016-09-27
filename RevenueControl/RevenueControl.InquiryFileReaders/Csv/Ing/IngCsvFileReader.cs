using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects.Entities;
using System.Globalization;
using CsvHelper;
using RevenueControl.DomainObjects;
using System.IO;
using System.Collections.Specialized;
using RevenueControl.Resource;

namespace RevenueControl.InquiryFileReaders.Csv.Ing
{
    public class IngCsvFileReader : ITransactionFileReader
    {

        public IList<Transaction> Read(string fileName, CultureInfo culture)
        {
            return Read(fileName, GlobalConstants.MaxPeriod, culture);
        }

        HybridDictionary CreateColumnMaps(CsvReader reader, CultureInfo culture)
        {
            HybridDictionary returnValue = new HybridDictionary();
            string date =Resources.ResourceManager.GetString("Date", culture);
            string transactionDetails = Resources.ResourceManager.GetString("TransactionDetails", culture);
            string debit = Resources.ResourceManager.GetString("Debit", culture);

            string credit = Resources.ResourceManager.GetString("Credit", culture);
            string[] headers = reader.FieldHeaders;
            for (int i = 0; i < headers.Length; i++)
            {
                if(headers[i] == date)
                {
                    returnValue["Date"] = i - 1;
                }
                else
                    if(headers[i] == transactionDetails)
                {
                    returnValue["TransactionDetails"] = i - 1;
                }
                else
                    if(headers[i] == debit)
                {
                    returnValue["Debit"] = i - 1;
                }
                else
                    if(headers[i] == credit)
                {
                    returnValue["Credit"] = i - 1;
                }
            }
            return returnValue;
        }

        public IList<Transaction> Read(string fileName, Period period, CultureInfo culture)
        {

            List<Transaction> returnValue = new List<Transaction>();
            List<CsvFileLine> lines = new List<CsvFileLine>();
            using (var sr = new StreamReader(fileName))
            {
                using (CsvReader reader = new CsvReader(sr))
                {
                    int counter = 0;
                    HybridDictionary maps = null;
                   
                    while(reader.Read())
                    {
                        if(maps == null)
                        {
                            maps = CreateColumnMaps(reader, culture);
                        }
                        counter++;
                        CsvFileLine fileLine = GenericCsvReader.CreateFileLine(reader, maps);
                        if (fileLine.Date != string.Empty && lines.Count > 0)
                        {
                            Transaction transaction = GenericCsvReader.GetTransactionFromLines(lines, culture);
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
                if(returnValue != null && lines.Count > 0)
                {
                    Transaction transaction = GenericCsvReader.GetTransactionFromLines(lines, culture);
                    if (transaction == null)
                    {
                        returnValue = null;
                    }
                    returnValue.Add(transaction);
                }
            }
            return returnValue;
        }
    }
}
