using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(iSynergy.Startup))]
namespace iSynergy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
