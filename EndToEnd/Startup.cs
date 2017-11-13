using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EndToEnd.Startup))]
namespace EndToEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
