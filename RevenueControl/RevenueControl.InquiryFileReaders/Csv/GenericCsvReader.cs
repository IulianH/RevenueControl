using RevenueControl.DomainObjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueControl.DomainObjects.Entities;
using System.Globalization;
using System.Collections.Specialized;
using CsvHelper;
using RevenueControl.Resources;
using RevenueControl.DomainObjects;
using System.IO;

namespace RevenueControl.InquiryFileReaders.Csv
{
    public class GenericCsvReader : ITransactionFileReader
    {
        static readonly string[] debitKeys = new string[] { "Debit" };
        static readonly string[] creditKeys = new string[] { "Credit" };
        static readonly string[] transactionDetailsKeys = new string[] { "TransactionDetails" };
        static readonly string[] transactionDateKeys = new string[] { "Date" };
        public enum FailReason
        {
            None = 0,
            NoDebitColumn = 1,
            NoCreditColumn = 2,
            NoTransactionDetailsColumn = 3,
            NoAmountColumn = 4,
            NoTransactionDateColumn = 5
        }

        static int GetIndexOfKey(string[] keys, string[] headers, CultureInfo culture)
        {
            var header = headers.Select((h, index) => new { header = h, index = index })
                .FirstOrDefault(h => keys.Select(key => Localization.GetResource(key, culture)).Any(resource => string.Compare(h.header, resource, true, culture) == 0));
            return header == null ? -1 : header.index;
        }


        internal static CsvFileLine CreateFileLine(CsvReader reader, HybridDictionary maps)
        {
            string[] currentRecord = reader.CurrentRecord;
            CsvFileLine returnValue = new CsvFileLine
            {
                CreditValue = currentRecord[(int)maps["Credit"]],
                Date = currentRecord[(int)maps["Date"]],
                DebitValue = currentRecord[(int)maps["Debit"]],
                TransactionDetails = currentRecord[(int)maps["TransactionDetails"]]
            };
            return returnValue;
        }


        HybridDictionary CreateColumnMaps(CsvReader reader, CultureInfo culture, out FailReason failReson)
        {
            HybridDictionary returnVal = null;
            failReson = FailReason.None;
            int debitIndex = GetIndexOfKey(debitKeys, reader.FieldHeaders, culture);
            if (debitIndex > -1)
            {
                int creditIndex = GetIndexOfKey(creditKeys, reader.FieldHeaders, culture);
                if (creditIndex > -1)
                {
                    int transactionDetailsIndex = GetIndexOfKey(transactionDetailsKeys, reader.FieldHeaders, culture);
                    if (transactionDetailsIndex > -1)
                    {

                        int transactionDateIndex = GetIndexOfKey(transactionDateKeys, reader.FieldHeaders, culture);
                        if (transactionDateIndex > -1)
                        {
                            returnVal = new HybridDictionary();
                            returnVal["Debit"] = debitIndex - 1;
                            returnVal["Credit"] = creditIndex - 1;
                            returnVal["TransactionDetails"] = transactionDetailsIndex - 1;
                            returnVal["Date"] = transactionDateIndex - 1;
                        }
                        else
                        {
                            failReson = FailReason.NoTransactionDateColumn;
                        }
                    }
                    else
                    {
                        failReson = FailReason.NoTransactionDetailsColumn;
                    }
                }
                else
                {
                    failReson = FailReason.NoCreditColumn;
                }
            }
            else
            {
                failReson = FailReason.NoDebitColumn;
            }
            return returnVal;
        }

        internal static Transaction GetTransactionFromLines(List<CsvFileLine> lines, CultureInfo culture)
        {

            Transaction returnValue = null;
            //validation
            if (lines.Any(line => line.Date != string.Empty) && lines.All(line => line.TransactionDetails != string.Empty)
            && lines.Count(line => line.Date != string.Empty && (line.DebitValue != string.Empty || line.CreditValue != string.Empty)) == 1)
            {
                CsvFileLine mainLine = lines[0];
                if (mainLine.DebitValue != string.Empty || mainLine.CreditValue != string.Empty && !(mainLine.DebitValue != string.Empty && mainLine.CreditValue != string.Empty))
                {
                    DateTime transactionDate;
                    TransactionType transactionType = mainLine.DebitValue != string.Empty ? TransactionType.Debit : TransactionType.Credit;
                    decimal amount;
                    string amountStr = transactionType == TransactionType.Debit ? mainLine.DebitValue : mainLine.CreditValue;
                    if (DateTime.TryParse(mainLine.Date, culture, DateTimeStyles.None, out transactionDate) && decimal.TryParse(amountStr, NumberStyles.Currency, culture, out amount))
                    {

                        if (amount > 0m)
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

        public IList<Transaction> Read(string fileName, CultureInfo culture)
        {
            return Read(fileName, GlobalConstants.MaxPeriod, culture);
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

                    while (reader.Read())
                    {
                        if (maps == null)
                        {
                            FailReason failReason = FailReason.None;
                            maps = CreateColumnMaps(reader, culture, out failReason);
                        }
                        counter++;
                        CsvFileLine fileLine = CreateFileLine(reader, maps);
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
                if (returnValue != null && lines.Count > 0)
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
