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
using RevenueControl.Resources;

namespace RevenueControl.InquiryFileReaders.Ing
{
    public class IngFileReader : ITransactionFileReader
    {
        class TransactionDetailsComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return String.Compare(x, y, CultureInfo.InvariantCulture, CompareOptions.Ordinal);
            }
        }

        public IList<Transaction> Read(string fileName, CultureInfo culture)
        {
            return Read(fileName, GlobalConstants.MaxPeriod, culture);
        }

        Transaction GetTransactionFromLines(List<IngFileLine> lines, CultureInfo culture)
        {
            
            Transaction returnValue = null;
            //validation
            if (lines.Any(line => line.Date != string.Empty) && lines.All(line => line.TransactionDetails != string.Empty)
            && lines.Count(line => line.Date != string.Empty && (line.DebitValue != string.Empty || line.CreditValue != string.Empty)) == 1)
            {
                IngFileLine mainLine = lines[0];
                if (mainLine.DebitValue != string.Empty || mainLine.CreditValue != string.Empty && !(mainLine.DebitValue != string.Empty && mainLine.CreditValue != string.Empty))
                {
                    DateTime transactionDate;
                    TransactionType transactionType = mainLine.DebitValue != string.Empty ? TransactionType.Debit : TransactionType.Credit;
                    decimal amount;
                    string amountStr = transactionType == TransactionType.Debit ? mainLine.DebitValue : mainLine.CreditValue;
                    if (DateTime.TryParse(mainLine.Date, culture, DateTimeStyles.None, out transactionDate) && decimal.TryParse(amountStr,NumberStyles.Currency, culture, out amount))
                    {
                        
                        if(amount > 0m)
                        {
                            Func<string[], string[]> sort = s1 => { Array.Sort(s1, new TransactionDetailsComparer()); return s1; };
                            returnValue = new Transaction
                            {
                                Amount = amount,
                                OtherDetails = string.Join(" ", sort(lines.Where(ln => ln != mainLine).Select(ln => ln.TransactionDetails).ToArray())),
                                TransactionDate = transactionDate,
                                TransactionDetails = mainLine.TransactionDetails,
                                TransactionType = transactionType
                            };
                        }
                    }
                }
            }
            return returnValue;
        }

        HybridDictionary CreateColumnMaps(CsvReader reader, CultureInfo culture)
        {
            HybridDictionary returnValue = new HybridDictionary();
            string date = Localization.GetDate(culture);
            string transactionDetails = Localization.GetTransactionDetails(culture);
            string debit = Localization.GetDebit(culture);
            string credit = Localization.GetCredit(culture);
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

        IngFileLine CreateFileLine(CsvReader reader, HybridDictionary maps)
        {
            string[] currentRecord = reader.CurrentRecord;
            IngFileLine returnValue = new IngFileLine
            {
                CreditValue = currentRecord[(int)maps["Credit"]],
                Date = currentRecord[(int)maps["Date"]],
                DebitValue = currentRecord[(int)maps["Debit"]],
                TransactionDetails = currentRecord[(int)maps["TransactionDetails"]]
            };
            return returnValue;
        }

        public IList<Transaction> Read(string fileName, Period period, CultureInfo culture)
        {

            List<Transaction> returnValue = new List<Transaction>();
            List<IngFileLine> lines = new List<IngFileLine>();
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
                        IngFileLine fileLine = CreateFileLine(reader, maps);
                        if (fileLine.Date != string.Empty && lines.Count > 0)
                        {
                            Transaction transaction = GetTransactionFromLines(lines, culture);
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
                    Transaction transaction = GetTransactionFromLines(lines, culture);
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
