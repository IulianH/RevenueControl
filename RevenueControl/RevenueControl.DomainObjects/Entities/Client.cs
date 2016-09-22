using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Entities
{
    public class Client
    {
        [Key]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
    