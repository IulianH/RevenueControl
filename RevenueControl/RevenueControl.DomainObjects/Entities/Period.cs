using System;

namespace RevenueControl.DomainObjects.Entities
{
    public struct Period
    {
        public Period(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate.Date;
            EndDate = endDate.Date;
        }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public bool Contains(DateTime dateTime)
        {
            return (dateTime >= StartDate) && (dateTime <= EndDate);
        }

        public override bool Equals(object obj)
        {
            if (obj is DateTime)
            {
                var other = (Period) obj;
                return (other.StartDate == StartDate) && (other.EndDate == EndDate);
            }
            return false;
        }

        public static bool operator ==(Period a, Period b)
        {
            return (a.StartDate == b.StartDate) && (a.EndDate == b.EndDate);
        }

        public static bool operator !=(Period a, Period b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return StartDate.GetHashCode() ^ EndDate.GetHashCode();
        }
    }
}