using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects
{
    public static class GlobalConstants
    {
        public static readonly Period MaxPeriod = new Period(new DateTime(1950, 1, 1),  DateTime.MaxValue);
    }
}
