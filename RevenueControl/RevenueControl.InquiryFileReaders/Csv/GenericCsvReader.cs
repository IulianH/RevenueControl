using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Resource;

namespace RevenueControl.InquiryFileReaders.Csv
{
    public class GenericCsvReader : ITransactionFileReader
    {
        public enum FailReason
        {
            None = 0,
            NoDebitColumn = 1,
            NoCreditColumn = 2,
            NoTransactionDetailsColumn = 3,
            NoAmountColumn = 4,
            NoTransactionDateColumn = 5
        }

        private static readonly string[] DebitKeys = {"Debit"};
        private static readonly string[] CreditKeys = {"Credit"};
        private static readonly string[] TransactionDetailsKeys = {"TransactionDetails"};
        private static readonly string[] TransactionDateKeys = {"Date"};

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
                    var counter = 0;
                    HybridDictionary maps = null;

                    while (reader.Read())
                    {
                        if (maps == null)
                        {
                            var failReason = FailReason.None;
                            maps = CreateColumnMaps(reader, culture, out failReason);
                        }
                        counter++;
                        var fileLine = CreateFileLine(reader, maps);
                        if ((fileLine.Date != string.Empty) && (lines.Count > 0))
                        {
                            var transaction = GetTransactionFromLines(lines, culture);
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
                    var transaction = GetTransactionFromLines(lines, culture);
                    if (transaction == null)
                        returnValue = null;
                    returnValue.Add(transaction);
                }
            }
            return returnValue;
        }

        private static int GetIndexOfKey(string[] keys, string[] headers, CultureInfo culture)
        {
            var header = headers.Select((h, index) => new {header = h, index})
                .FirstOrDefault(
                    h =>
                        keys.Select(key => Resources.ResourceManager.GetString(key, culture))
                            .Any(resource => string.Compare(h.header, resource, true, culture) == 0));
            return header?.index ?? -1;
        }


        internal static CsvFileLine CreateFileLine(CsvReader reader, HybridDictionary maps)
        {
            var currentRecord = reader.CurrentRecord;
            var returnValue = new CsvFileLine
            {
                CreditValue = currentRecord[(int) maps["Credit"]],
                Date = currentRecord[(int) maps["Date"]],
                DebitValue = currentRecord[(int) maps["Debit"]],
                TransactionDetails = currentRecord[(int) maps["TransactionDetails"]]
            };
            return returnValue;
        }


        private static HybridDictionary CreateColumnMaps(ICsvReader reader, CultureInfo culture, out FailReason failReson)
        {
            HybridDictionary returnVal = null;
            failReson = FailReason.None;
            var debitIndex = GetIndexOfKey(DebitKeys, reader.FieldHeaders, culture);
            if (debitIndex > -1)
            {
                var creditIndex = GetIndexOfKey(CreditKeys, reader.FieldHeaders, culture);
                if (creditIndex > -1)
                {
                    var transactionDetailsIndex = GetIndexOfKey(TransactionDetailsKeys, reader.FieldHeaders, culture);
                    if (transactionDetailsIndex > -1)
                    {
                        var transactionDateIndex = GetIndexOfKey(TransactionDateKeys, reader.FieldHeaders, culture);

                        if (transactionDateIndex > -1)
                        {
                            //stupid bug in ING
                            var noOffset = (transactionDateIndex == 0) || (debitIndex == 0) || (creditIndex == 0) ||
                                           (transactionDetailsIndex == 0);

                            returnVal = new HybridDictionary
                            {
                                ["Debit"] = noOffset ? debitIndex : debitIndex - 1,
                                ["Credit"] = noOffset ? creditIndex : creditIndex - 1,
                                ["TransactionDetails"] = noOffset
                                    ? transactionDetailsIndex
                                    : transactionDetailsIndex - 1,
                                ["Date"] = noOffset ? transactionDateIndex : transactionDateIndex - 1
                            };
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
            if (lines.All(line => line.Date == string.Empty) ||
                lines.Any(line => line.TransactionDetails == string.Empty) || (lines.Count(
                                                                                    line =>
                                                                                        (line.Date != string.Empty) &&
                                                                                        ((line.DebitValue !=
                                                                                          string.Empty) ||
                                                                                         (line.CreditValue !=
                                                                                          string.Empty))) != 1))
                return null;
            var mainLine = lines[0];
            if ((mainLine.DebitValue != string.Empty) ||
                ((mainLine.CreditValue != string.Empty) &&
                 !((mainLine.DebitValue != string.Empty) && (mainLine.CreditValue != string.Empty))))
            {
                DateTime transactionDate;
                var transactionType = mainLine.DebitValue != string.Empty
                    ? TransactionType.Debit
                    : TransactionType.Credit;
                decimal amount;
                var amountStr = transactionType == TransactionType.Debit
                    ? mainLine.DebitValue
                    : mainLine.CreditValue;
                if (DateTime.TryParse(mainLine.Date, culture, DateTimeStyles.None, out transactionDate) &&
                    decimal.TryParse(amountStr, NumberStyles.Currency, culture, out amount))
                    if (amount > 0m)
                    {
                        Func<string[], string[]> sort = s1 =>
                        {
                            Array.Sort(s1, new TransactionDetailsComparer());
                            return s1;
                        };
                        returnValue = new Transaction
                        {
                            Amount = amount,
                            OtherDetails =
                                string.Join(" ",
                                    sort(
                                        lines.Where(ln => ln != mainLine)
                                            .Select(ln => ln.TransactionDetails)
                                            .ToArray())),
                            TransactionDate = transactionDate,
                            TransactionDetails = mainLine.TransactionDetails,
                            TransactionType = transactionType
                        };
                    }
            }
            return returnValue;
        }
    }
}