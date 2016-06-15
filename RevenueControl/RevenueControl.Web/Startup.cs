using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RevenueControl.Web.Startup))]
namespace RevenueControl.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
