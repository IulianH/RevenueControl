using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace RevenueControl.InquiryFileReaders.Ing
{
    public sealed class IngFileLineMap : CsvClassMap<IngFileLine>
    {
        public IngFileLineMap()
        {
            Map(m => m.Date).Index(0);
            Map(m => m.DateTransactionDetailsColSeparator).Index(1);
            Map(m => m.TransactionDetails).Index(2);
            Map(m => m.TransactonDetailsDebitValueColSeparator).Index(3);
            Map(m => m.DebitValue).Index(4);
            Map(m => m.DebitCreditColSeparator).Index(5);
            Map(m => m.CreditValue).Index(6);
            Map(m => m.EndingEmptySpace).Index(7);
        }
    }

    public class IngFileLine 
    {
        public string Date { get; set; }

        public string DateTransactionDetailsColSeparator { get; set; }

        public string TransactionDetails { get; set; }

        public string TransactonDetailsDebitValueColSeparator { get; set; }

        public string DebitValue { get; set; }

        public string DebitCreditColSeparator { get; set; }

        public string CreditValue { get; set; }

        public string EndingEmptySpace { get; set; }
    }
}
