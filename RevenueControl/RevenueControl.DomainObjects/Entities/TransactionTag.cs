﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Entities
{
    public class TransactionTag
    {
        public int Id { get; set; }

        public string ClientName { get; set;}

        public int TransactionId { get; set; }

        public string Tag { get; set; }
    }
}