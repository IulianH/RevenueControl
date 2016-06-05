using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace RevenueControl.InquiryFileReaders.Csv
{
  
    public class CsvFileLine 
    {
        public string Date { get; set; }


        public string TransactionDetails { get; set; }


        public string DebitValue { get; set; }


        public string CreditValue { get; set; }

    }
}
