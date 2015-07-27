using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ct.Web.Startup))]
namespace ct.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
