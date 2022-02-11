using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TripApplication.Startup))]
namespace TripApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
