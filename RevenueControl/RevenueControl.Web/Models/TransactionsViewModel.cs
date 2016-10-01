using System.Collections.Generic;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.Web.Models
{
    public class TransactionsViewModel
    {
        public int DataSourceId { get; set; }

        public string DataSource { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }
    }
}