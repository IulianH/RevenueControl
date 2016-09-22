using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RevenueControl.DomainObjects.Entities;

namespace RevenueControl.Web.Context
{
    public class RevenueControlContext : IRevenueControlContext
    {
        public string LoggedInClient
        {
            get
            {
                return "DefaultClient";
            }
        }

        
    }
}