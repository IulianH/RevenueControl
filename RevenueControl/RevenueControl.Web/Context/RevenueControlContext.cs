namespace RevenueControl.Web.Context
{
    public class RevenueControlContext : IRevenueControlContext
    {
        public string LoggedInClient
        {
            get { return "DefaultClient"; }
        }
    }
}