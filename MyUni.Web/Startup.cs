using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyUni.Web.Startup))]
namespace MyUni.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
