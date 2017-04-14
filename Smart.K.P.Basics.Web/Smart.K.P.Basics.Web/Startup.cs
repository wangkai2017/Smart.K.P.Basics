using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Smart.K.P.Basics.Web.Startup))]
namespace Smart.K.P.Basics.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
