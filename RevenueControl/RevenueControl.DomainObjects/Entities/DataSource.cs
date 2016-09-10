using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Entities
{ 
    public class DataSource
    {
       
        public string BankAccount { get; set; }

        public string Name { get; set; }

        public string Culture { get; set; }

        public string ClientName { get; set; }

        public int Id { get; set; }
    }
}
