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

        public int Id { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
