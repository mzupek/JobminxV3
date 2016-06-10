using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JobminxV3.Startup))]
namespace JobminxV3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
