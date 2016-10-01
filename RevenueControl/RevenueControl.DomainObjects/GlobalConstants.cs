using System;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.DomainObjects
{
    public static class GlobalConstants
    {
        public static readonly Period MaxPeriod = new Period(new DateTime(1950, 1, 1), DateTime.MaxValue);
    }
}