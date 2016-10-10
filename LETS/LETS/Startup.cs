using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LETS.Startup))]
namespace LETS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        private void ConfigureAuth(IAppBuilder app)
        {

        }
    }
}
