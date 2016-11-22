using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiniAbp.WebTest.Startup))]
namespace MiniAbp.WebTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
