using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.DomainObjects.Entities
{
    public struct Period
    {
        private DateTime _startDate;
        private DateTime _endDate;

        public Period(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate.Date;
            _endDate = endDate.Date;
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
        }
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
        }

        public bool Contains(DateTime dateTime)
        {
            return dateTime >= StartDate && dateTime <= EndDate;
        }

        public override bool Equals(object obj)
        {
           if(obj is DateTime)
           {
                Period other = (Period)obj;
                return other.StartDate == StartDate && other.EndDate == EndDate;
           }
            return false;
        }

        public static bool operator ==(Period a, Period b)
        {
            return a.StartDate == b.StartDate && a.EndDate == b.EndDate;
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
