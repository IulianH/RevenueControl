using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<DataSource> DataSources { get; set; }
    }
}
