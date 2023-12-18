using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MapfreHSBC.Startup))]
namespace MapfreHSBC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }


    }
}
