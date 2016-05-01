using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Entities
{ 
    public class DataSource
    {
        public string BankAccount { get; set; }

        public string Name { get; set; }

        public virtual IList<Transaction> Transactions { get; set; }

        public int DataSourceId { get; set; }

        public IEnumerable<string> Cards { get; set; }

        public string BankName { get; set; }

        public string FileReaderId { get; set; }
    }
}
