using Microsoft.Owin;
using Owin;
using RevenueControl.Web;

[assembly: OwinStartup(typeof(Startup))]

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