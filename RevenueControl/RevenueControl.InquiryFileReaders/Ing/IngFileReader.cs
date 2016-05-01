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

namespace RevenueControl.InquiryFileReaders.Ing
{
    public class IngFileReader : ITransactionFileReader
    {
        public bool Success
        {
            get
            {
                throw new NotImplementedException();
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
            if (lines.Any(line => line.Date != string.Empty) && lines.All(line => line.TransactionDetails != string.Empty && line.DateTransactionDetailsColSeparator == string.Empty &&
            line.TransactonDetailsDebitValueColSeparator == string.Empty && line.DebitCreditColSeparator == string.Empty && line.EndingEmptySpace == string.Empty)
            && lines.Count(line => line.Date != string.Empty && (line.DebitValue != string.Empty || line.CreditValue != string.Empty)) == 1)
            {
                IngFileLine mainLine = lines.Single(ln => ln.Date != string.Empty);
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
                            returnValue = new Transaction
                            {
                                Amount = amount,
                                OtherDetails = string.Join(" ", lines.Where(ln => ln != mainLine).Select(ln => ln.TransactionDetails)),
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



        public IList<Transaction> Read(string fileName, Period period, CultureInfo culture)
        {

            List<Transaction> returnValue = new List<Transaction>();
            List<IngFileLine> lines = new List<IngFileLine>();
            using (var sr = new StreamReader(fileName))
            {

                var reader = new CsvReader(sr);
                reader.Configuration.RegisterClassMap<IngFileLineMap>();
                int counter = 0;
                foreach (IngFileLine fileLine in reader.GetRecords<IngFileLine>())
                {
                    counter++;
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
