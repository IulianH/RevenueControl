using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevenueControl.Web.Models
{
    public class TransactionsViewModel
    {
        public string DataSource { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }
    }
}