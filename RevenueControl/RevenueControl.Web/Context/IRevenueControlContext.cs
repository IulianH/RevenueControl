using RevenueControl.DomainObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.Web.Context
{
    public interface IRevenueControlContext
    {
        string LoggedInClient { get; }

    }
}
