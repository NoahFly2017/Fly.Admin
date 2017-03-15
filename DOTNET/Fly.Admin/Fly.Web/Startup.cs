using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Fly.Web.Startup))]
namespace Fly.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
